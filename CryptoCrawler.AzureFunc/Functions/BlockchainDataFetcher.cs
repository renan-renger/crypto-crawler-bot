using System;
using CryptoCrawler.Contracts.Domain;
using CryptoCrawler.Application.Services;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using CryptoCrawler.Infrastructure.Extensions;
using CryptoCrawler.Contracts.Messaging.Command;

namespace CryptoCrawler.AzureFunc.Functions
{
    public class BlockchainDataFetcher
    {
        private readonly IApiCrawler<BlockchainInfoDomain> _crawler;
        private readonly IProcessScrapedDataBuilder _builder;
        private readonly IMessageSender<ProcessScrapedData> _sender;

        public BlockchainDataFetcher(IApiCrawler<BlockchainInfoDomain> crawler, IProcessScrapedDataBuilder builder, IMessageSender<ProcessScrapedData> sender)
        {
            _crawler = crawler;
            _builder = builder;
            _sender = sender;
        }

        [FunctionName("BlockchainDataFetcher")]
        public void Run([TimerTrigger("0 */10 * * * *")]TimerInfo myTimer, ILogger log)
        {
            try
            {
                if (!_crawler.SetupCrawler()) return;

                _sender.SendCommand(_builder.BuildCommand(new List<object> { _crawler.Fetch() }, _crawler.ExposeEndpoint()));

                log.LogInformation($"BlockchainDataFetcher ran to completion @ {DateTime.UtcNow.AsUtc()}\n");
            }
            catch (Exception ex)
            {
                log.LogError($"An error has occured @ {DateTime.UtcNow.AsUtc()} \n Message: {ex.Message} \n Stack Trace: {ex.StackTrace} \n Source: {ex.Source}");
            }
        }
    }
}
