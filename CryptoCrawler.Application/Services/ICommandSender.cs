namespace CryptoCrawler.Application.Services
{
    public interface ICommandSender<T>
    {
        void SendCommand(T command);
    }
}
