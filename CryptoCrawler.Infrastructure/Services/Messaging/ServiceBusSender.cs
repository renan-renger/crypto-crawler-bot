using CryptoCrawler.Application.Services;
using CryptoCrawler.Contracts.Messaging.Command;
using Microsoft.ServiceBus.Messaging;

namespace CryptoCrawler.Infrastructure.Services.Messaging
{
    public class ServiceBusSender : ICommandSender<ProcessScrapedData>
    {
        private const string connectionString = "";
        private const string queueName = "";

        public void SendCommand(ProcessScrapedData command)
        {
            var _client = QueueClient.CreateFromConnectionString(connectionString, queueName);
            BrokeredMessage message = new BrokeredMessage(command);
            _client.SendAsync(message).Wait();
        }
    }
}
