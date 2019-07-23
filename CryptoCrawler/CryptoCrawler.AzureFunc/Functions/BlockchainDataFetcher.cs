using System;
using CryptoCrawler.Common.Crawlers;
using CryptoCrawler.Common.Interfaces;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using RestSharp;

namespace CryptoCrawler.AzureFunc.Functions
{
    public class BlockchainDataFetcher
    {
        private readonly IApiCrawler _crawler;

        public BlockchainDataFetcher(IApiCrawler crawler)
        {
            _crawler = crawler;
        }

        [FunctionName("BlockchainDataFetcher")]
        public void Run([TimerTrigger("0 */1 * * * *")]TimerInfo myTimer, ILogger log)
        {

            if (_crawler.SetupCrawler("https://blockchain.info/q", Method.GET, "hashrate"))
                log.LogInformation($"C# Timer trigger function executed at: {DateTime.Now} with crawled hashrate: {_crawler.Fetch()}\n");

            if (_crawler.SetupCrawler("https://blockchain.info/q", Method.GET, "getdifficulty"))
                log.LogInformation($"C# Timer trigger function executed at: {DateTime.Now} with crawled difficulty: {_crawler.Fetch()}\n");
        }
    }
}
