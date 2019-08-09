using System.Threading.Tasks;

namespace CryptoCrawler.Application.Services
{
    public interface IMessageSender<T>
    {
        Task SendCommand(T command);

        Task SendEvent(T eventData);
    }
}
