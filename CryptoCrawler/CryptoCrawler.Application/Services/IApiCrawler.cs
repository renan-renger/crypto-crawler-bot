using System.Collections.Generic;
using RestSharp.Authenticators;

namespace CryptoCrawler.Application.Services
{
    public interface IApiCrawler<T>
    {
        bool SetupCrawler(IAuthenticator authToken = null);
        IList<T> Fetch();
    }
}
