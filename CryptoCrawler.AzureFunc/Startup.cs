using Microsoft.Azure.Functions.Extensions.DependencyInjection;

[assembly: FunctionsStartup(typeof(CryptoCrawler.AzureFunc.Startup))]

namespace CryptoCrawler.AzureFunc
{
    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            new Infrastructure.IoC.Services().Configure(builder.Services);
        }
    }
}
