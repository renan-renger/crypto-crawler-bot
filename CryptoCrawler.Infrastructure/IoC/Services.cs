using CryptoCrawler.Contracts.Domain;
using CryptoCrawler.Application.Services;
using CryptoCrawler.Infrastructure.Services.Crawlers;
using Microsoft.Extensions.DependencyInjection;

namespace CryptoCrawler.Infrastructure.IoC
{
    public class Services
    {
        public void Configure(IServiceCollection services)
        {
            services.AddScoped<IApiCrawler<BlockchainInfoDomain>, BlockchainInfoCrawler>();
        }
    }
}
