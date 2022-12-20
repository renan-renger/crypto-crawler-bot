using System;
using System.Collections.Generic;
using CryptoCrawler.Application.Services;
using CryptoCrawler.Contracts.Domain;
using CryptoCrawler.Infrastructure.Extensions;
using CryptoCrawler.InternalContracts.Domain;
using CryptoCrawler.InternalContracts.SenderTypes;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using RestSharp.Authenticators;

namespace CryptoCrawler.AzureFunc.Functions
{
    public class MercadoBitcoinFetcher
    {
        private readonly IApiCrawler<List<MercadoBitcoinTicker>> _crawler;
        private readonly IApiCrawler<MercadoBitcoinAuthorizationResponse> _auth;
        private readonly IProcessScrapedDataBuilder _builder;
        private readonly IQueueClient<IAzureServiceBusQueueType> _queueClient;
        private readonly ITopicClient<IAzureServiceBusTopicType> _topicClient;
        private static MercadoBitcoinAuthorizationResponse _authToken;

        private readonly string _queueName = "PendingProcessing";
        private readonly string _topicName = "ScrappedData";

        public MercadoBitcoinFetcher(IApiCrawler<List<MercadoBitcoinTicker>> crawler, IApiCrawler<MercadoBitcoinAuthorizationResponse> auth, IProcessScrapedDataBuilder builder, IQueueClient<IAzureServiceBusQueueType> queueClient, ITopicClient<IAzureServiceBusTopicType> topicClient)
        {
            _crawler = crawler;
            _auth = auth;
            _builder = builder;
            _queueClient = queueClient;
            _topicClient = topicClient;
        }

        [FunctionName("MercadoBitcoinFetcher")]
        public void Run([TimerTrigger("0 */5 * * * *")]TimerInfo myTimer, ILogger log)
        {
            try
            {
                Authorize(log);

                if (!_crawler.SetupCrawler(new JwtAuthenticator(_authToken.AccessToken))) return;            

                var payload = _builder.BuildCommand(new List<object> { _crawler.Fetch() }, _crawler.ExposeEndpoint());

                _queueClient.SendMessage(payload, _queueName);
                _topicClient.PublishToSubscription(payload, _topicName);

                log.LogInformation($"BlockchainDataFetcher ran to completion @ {DateTime.UtcNow.AsUtc()}\n");
            }
            catch (Exception ex)
            {
                log.LogError($"An error has occured @ {DateTime.UtcNow.AsUtc()} \n Message: {ex.Message} \n Stack Trace: {ex.StackTrace} \n Source: {ex.Source}");
            }
        }

        internal void Authorize(ILogger log)
        {
            try
            {
                if (_authToken != null && !string.IsNullOrWhiteSpace(_authToken.AccessToken) && _authToken.Expiration < DateTime.Now) { return; }

                if (!_auth.SetupCrawler()) throw new UnauthorizedAccessException();

                var authToken = _auth.Fetch();

                if (authToken == null || string.IsNullOrWhiteSpace(authToken.AccessToken)) throw new UnauthorizedAccessException();

                _authToken = authToken;
            }
            catch (Exception ex)
            {
                log.LogError($"An error has occured @ {DateTime.UtcNow.AsUtc()} \n Message: {ex.Message} \n Stack Trace: {ex.StackTrace} \n Source: {ex.Source}");
            }
        }
    }
}
