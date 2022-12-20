using System;
using System.Threading.Tasks;
using CryptoCrawler.Application.Services;
using Microsoft.Extensions.Configuration;
using Azure.Identity;
using Azure.Messaging.ServiceBus;
using CryptoCrawler.InternalContracts.SenderTypes;
using Newtonsoft.Json;
using Azure.Messaging.ServiceBus.Administration;
using Azure.Core;

namespace CryptoCrawler.Infrastructure.Services.Messaging
{
    public class AzureServiceBusQueueClient : IQueueClient<IAzureServiceBusQueueType>, IDisposable
    {
        private readonly int MessageExpiryHours;
        private readonly int MessageExpiryHoursDefault = 12;

        private static ServiceBusClient _client;
        private static ServiceBusAdministrationClient _adminClient;
        private static ServiceBusReceiver _receiver;
        private static ServiceBusSender _sender;
        private readonly ServiceBusClientOptions _clientOptions = new()
        {
            TransportType = ServiceBusTransportType.AmqpWebSockets,
            RetryOptions = new ServiceBusRetryOptions() { 
                MaxRetries = 10, 
                Delay = TimeSpan.FromSeconds(3), 
                MaxDelay = TimeSpan.FromSeconds(60), 
                Mode = ServiceBusRetryMode.Exponential
            }
        };

        public AzureServiceBusQueueClient(IConfiguration configuration)
        {
            TokenCredential azureCredential = null;
            string serviceBusConnString = configuration["ServiceBusConnectionString"];

            if (string.IsNullOrWhiteSpace(serviceBusConnString)) {
                var credentialOptions = new DefaultAzureCredentialOptions() { TenantId = configuration["TenantId"], ManagedIdentityClientId = configuration["UserAssignedClientId"] };
                azureCredential = new DefaultAzureCredential(credentialOptions);
            }

            _client = azureCredential == null ? new ServiceBusClient(serviceBusConnString, _clientOptions) : new ServiceBusClient(configuration["ServiceBusNamespace"], azureCredential, _clientOptions);
            _adminClient = azureCredential == null ? new ServiceBusAdministrationClient(serviceBusConnString) : new ServiceBusAdministrationClient(configuration["ServiceBusNamespace"], azureCredential);

            MessageExpiryHours = int.TryParse(configuration["MessageExpiryHours"], out int temp) ? temp : MessageExpiryHoursDefault;
        }

        public async Task CreateQueue(string queueName)
        {
            var topicExists = (await _adminClient.QueueExistsAsync(queueName)).Value;
            if (topicExists) { return; }
            await _adminClient.CreateQueueAsync(queueName);
        }

        public async Task DeleteQueue(string queueName)
        {
            var topicExists = (await _adminClient.QueueExistsAsync(queueName)).Value;
            if (!topicExists) { return; }
            await _adminClient.CreateQueueAsync(queueName);
        }

        public async Task SendMessage(object payloadObject, string queueName)
        {
            _sender ??= _client.CreateSender(queueName);

            ServiceBusMessage serviceBusMessage = new ServiceBusMessage
            {
                ContentType = "application/json",
                Body = BinaryData.FromString(JsonConvert.SerializeObject(payloadObject)),
                TimeToLive = TimeSpan.FromHours(MessageExpiryHours)
            };
            serviceBusMessage.ApplicationProperties.Add("AssemblyQualifiedName", payloadObject.GetType().AssemblyQualifiedName);

            await _sender.SendMessageAsync(serviceBusMessage);
        }

        public async Task<object> ReceiveMessage(string queueName)
        {
            var queueExists = (await _adminClient.QueueExistsAsync(queueName)).Value;

            if (queueExists)
            {
                _receiver ??= _client.CreateReceiver(queueName);

                var message = await _receiver.ReceiveMessageAsync();

                if (string.IsNullOrWhiteSpace(message.ApplicationProperties["AssemblyQualifiedName"].ToString()))
                {
                    await _receiver.DeadLetterMessageAsync(message, "AssemblyQualifiedName property unavailable in message.");
                }

                try
                {
                    var bodyType = Type.GetType(message.ApplicationProperties["AssemblyQualifiedName"].ToString());
                    var bodyPayload = JsonConvert.DeserializeObject(message.Body.ToString(), bodyType);
                    await _receiver.CompleteMessageAsync(message);
                    return bodyPayload;
                }
                catch
                {
                    await _receiver.DeadLetterMessageAsync(message, "Cannot parse back into dotnet object.");
                    throw new Exception("Cannot parse back into dotnet object.");
                }
            }

            return null;
        }

        public async void Dispose()
        {
            if (_receiver != null) { await _receiver.DisposeAsync(); }
            if (_sender != null) { await _sender.DisposeAsync();}
            if (_client != null) { await _client.DisposeAsync();}
        }
    }
}
