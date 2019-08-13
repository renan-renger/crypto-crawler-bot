using RestSharp.Authenticators;

namespace CryptoCrawler.Application.Services
{
    public interface IApiCrawler<out T>
    {
        bool SetupCrawler(IAuthenticator authToken = null);
        T Fetch();
        string ExposeEndpoint();
    }
}
