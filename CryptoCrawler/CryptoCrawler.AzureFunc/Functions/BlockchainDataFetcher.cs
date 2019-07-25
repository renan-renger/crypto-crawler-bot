using System;
using CryptoCrawler.Application.Domain;
using CryptoCrawler.Application.Services;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace CryptoCrawler.AzureFunc.Functions
{
    public class BlockchainDataFetcher
    {
        private readonly IApiCrawler<BlockchainInfoDomain> _crawler;

        public BlockchainDataFetcher(IApiCrawler<BlockchainInfoDomain> crawler)
        {
            _crawler = crawler;
        }

        [FunctionName("BlockchainDataFetcher")]
        public void Run([TimerTrigger("0 */5 * * * *")]TimerInfo myTimer, ILogger log)
        {
            if (!_crawler.SetupCrawler()) return;

            log.LogInformation($"C# Timer trigger function executed at: {DateTime.Now} with crawled data:\n");
            log.LogInformation($"\n\n{JsonConvert.SerializeObject(_crawler.Fetch())}\n\n");
        }
    }
}
