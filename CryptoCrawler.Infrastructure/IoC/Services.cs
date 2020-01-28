using CryptoCrawler.Contracts.Domain;
using CryptoCrawler.Application.Services;
using CryptoCrawler.Infrastructure.Services.Crawlers;
using Microsoft.Extensions.DependencyInjection;
using CryptoCrawler.Infrastructure.Services.Builders;
using CryptoCrawler.Contracts.Messaging.Command;
using CryptoCrawler.Infrastructure.Services.Messaging;
using CryptoCrawler.InternalContracts.SenderTypes;

namespace CryptoCrawler.Infrastructure.IoC
{
    public class Services
    {
        public void Configure(IServiceCollection services)
        {
            services.AddScoped<IApiCrawler<BlockchainInfoDomain>, BlockchainInfoCrawler>();
            services.AddScoped<IProcessScrapedDataBuilder, ProcessScrapedDataBuilder>();
            services.AddScoped<IMessageSender<object, IAzureServiceBusType>, ServiceBusSender>();
            services.AddScoped<IMessageReceiver<IAzureServiceBusType>, ServiceBusReceiver>();
        }
    }
}
