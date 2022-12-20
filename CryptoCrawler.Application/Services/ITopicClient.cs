using System.Threading.Tasks;

namespace CryptoCrawler.Application.Services
{
    public interface ITopicClient<TSender>
    {
        Task CreateTopic(string topicName);
        Task DeleteTopic(string topicName);
        Task CreateSubscription (string topicName, string subscriptionName);
        Task DeleteSubscription(string topicName, string subscriptionName);
        Task PublishToSubscription(object payloadObject, string topicName);
        Task<object> SubscribeToSubscription(string topicName, string subscriptionName);
    }
}
