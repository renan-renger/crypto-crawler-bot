using System.Threading.Tasks;

namespace CryptoCrawler.Application.Services
{
    public interface IMessageSender<TMessage, TSender>
    {
        Task SendCommand(TMessage command);

        Task SendEvent(TMessage eventData);
    }
}
