using System;
using System.Text;
using System.Threading.Tasks;
using CryptoCrawler.Application.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Azure.ServiceBus;
using Newtonsoft.Json;
using CryptoCrawler.InternalContracts.SenderTypes;

namespace CryptoCrawler.Infrastructure.Services.Messaging
{
    public class ServiceBusSender : IMessageSender<object, IAzureServiceBusType>
    {
        private static QueueClient _queueClient;
        private static TopicClient _topicClient;

        public ServiceBusSender(IConfiguration configuration)
        {
            _queueClient = new QueueClient(
                configuration["ServiceBusConnectionString"],
                configuration["queueName"]);

            _topicClient = new TopicClient(
                configuration["ServiceBusConnectionString"],
                configuration["topicName"]);
        }

        public async Task SendCommand(object command)
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

        public async Task SendEvent(object eventData)
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
