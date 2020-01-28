using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;

namespace CryptoCrawler.AzureFunc.Functions
{
    public static class ColdStorageHandler
    {
        [FunctionName("ColdStorageHandler")]
        public static void Run([ServiceBusTrigger("fetchedData", "ColdStorage",  Connection = "ServiceBusConnectionString")]string myQueueItem, ILogger log)
        {
            log.LogInformation($"C# ServiceBus queue trigger function processed message: {myQueueItem}");
        }
    }
}
