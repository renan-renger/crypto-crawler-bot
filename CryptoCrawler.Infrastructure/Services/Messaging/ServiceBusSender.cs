using System;
using System.Text;
using System.Threading.Tasks;
using CryptoCrawler.Application.Services;
using CryptoCrawler.Contracts.Messaging.Command;
using Microsoft.Extensions.Configuration;
using Microsoft.Azure.ServiceBus;
using Newtonsoft.Json;

namespace CryptoCrawler.Infrastructure.Services.Messaging
{
    public class ServiceBusSender : IMessageSender<ProcessScrapedData>
    {
        private readonly QueueClient _queueClient;
        private readonly TopicClient _topicClient;
        private const string queueName = "pendingProcess";

        public ServiceBusSender(IConfiguration configuration)
        {
            _queueClient = new QueueClient(
                Environment.GetEnvironmentVariable("ServiceBusConnectionString"),
                queueName);
            _topicClient = new TopicClient(
                Environment.GetEnvironmentVariable("ServiceBusConnectionString"),
                queueName);
        }

        public async Task SendCommand(ProcessScrapedData command)
        {
            try
            {
                string data = JsonConvert.SerializeObject(command);
                Message message = new Message
                {
                    Body = Encoding.UTF8.GetBytes(data),
                    ContentType = "application/json",
                    MessageId = Guid.NewGuid().ToString()
                };

                await _queueClient.SendAsync(message);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        public async Task SendEvent(ProcessScrapedData eventData)
        {
            
            try
            {
                string data = JsonConvert.SerializeObject(eventData);
                Message message = new Message
                {
                    Body = Encoding.UTF8.GetBytes(data),
                    ContentType = "application/json",
                    MessageId = Guid.NewGuid().ToString()
                };

                await _topicClient.SendAsync(message);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
    }
}
