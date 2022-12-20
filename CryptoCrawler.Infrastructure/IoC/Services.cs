using CryptoCrawler.Contracts.Domain;
using CryptoCrawler.Application.Services;
using CryptoCrawler.Infrastructure.Services.Crawlers;
using Microsoft.Extensions.DependencyInjection;
using CryptoCrawler.Infrastructure.Services.Builders;
using CryptoCrawler.Infrastructure.Services.Messaging;
using CryptoCrawler.InternalContracts.SenderTypes;
using System.Collections.Generic;
using CryptoCrawler.InternalContracts.Domain;

namespace CryptoCrawler.Infrastructure.IoC
{
    public class Services
    {
        public void Configure(IServiceCollection services)
        {
            services.AddScoped<IApiCrawler<BlockchainInfoDomain>, BlockchainInfoCrawler>();
            services.AddScoped<IProcessScrapedDataBuilder, ProcessScrapedDataBuilder>();
            services.AddSingleton<IQueueClient<IAzureServiceBusQueueType>, AzureServiceBusQueueClient>();
            services.AddSingleton<ITopicClient<IAzureServiceBusTopicType>, AzureServiceBusTopicClient>();
            services.AddScoped<IApiCrawler<List<MercadoBitcoinTicker>>, MercadoBitcoinTickerCrawler>();
            services.AddScoped<IApiCrawler<MercadoBitcoinAuthorizationResponse>, MercadoBitcoinAuthorizationCrawler>();
        }
    }
}
