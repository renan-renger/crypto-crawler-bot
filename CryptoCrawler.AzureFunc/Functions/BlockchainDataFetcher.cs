using System;
using CryptoCrawler.Contracts.Domain;
using CryptoCrawler.Application.Services;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using CryptoCrawler.Infrastructure.Extensions;
using CryptoCrawler.InternalContracts.SenderTypes;
using System.Runtime.CompilerServices;

namespace CryptoCrawler.AzureFunc.Functions
{
    public class BlockchainDataFetcher
    {
        private readonly IApiCrawler<BlockchainInfoDomain> _crawler;
        private readonly IProcessScrapedDataBuilder _builder;
        private readonly IQueueClient<IAzureServiceBusQueueType> _queueClient;
        private readonly ITopicClient<IAzureServiceBusTopicType> _topicClient;

        private readonly string _queueName = "PendingProcessing";
        private readonly string _topicName = "ScrappedData";

        public BlockchainDataFetcher(IApiCrawler<BlockchainInfoDomain> crawler, IProcessScrapedDataBuilder builder, IQueueClient<IAzureServiceBusQueueType> queueClient, ITopicClient<IAzureServiceBusTopicType> topicClient)
        {
            _crawler = crawler;
            _builder = builder;
            _queueClient = queueClient;
            _topicClient = topicClient;

            _queueClient.CreateQueue(_queueName).Wait();
            _topicClient.CreateTopic(_topicName).Wait();            
        }

        [FunctionName("BlockchainDataFetcher")]
        public void Run([TimerTrigger("0 */5 * * * *")]TimerInfo myTimer, ILogger log)
        {
            try
            {
                if (!_crawler.SetupCrawler()) return;

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
    }
}
