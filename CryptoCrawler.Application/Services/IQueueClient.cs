using System.Threading.Tasks;

namespace CryptoCrawler.Application.Services
{
    public interface IQueueClient<TSender>
    {
        Task CreateQueue(string queueName);
        Task DeleteQueue(string queueName);
        Task SendMessage(object payloadObject, string queueName);
        Task<object> ReceiveMessage(string queueName);
    }
}
