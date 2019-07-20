using System;
using CryptoCrawler.Common.Crawlers;
using CryptoCrawler.Common.Interfaces;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using RestSharp;

[assembly: FunctionsStartup(typeof(CryptoCrawler.AzureFunc.Startup))]

namespace CryptoCrawler.AzureFunc
{
    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            builder.Services.AddSingleton<IApiCrawler, ApiCrawler>();
        }
    }
}
