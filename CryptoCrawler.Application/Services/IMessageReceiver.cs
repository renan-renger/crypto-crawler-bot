using System.Threading.Tasks;

namespace CryptoCrawler.Application.Services
{
    public interface IMessageReceiver<TSender>
    {
        void ReceiveCommand(Task handlerFunction);

        void ReceiveEvent(Task handlerFunction);
    }
}
