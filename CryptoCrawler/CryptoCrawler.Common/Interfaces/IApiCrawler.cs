using System;
using System.Collections.Generic;
using System.Text;
using RestSharp;
using RestSharp.Authenticators;

namespace CryptoCrawler.Common.Interfaces
{
    public interface IApiCrawler
    {
        bool SetupCrawler(string endpointAddress, Method method = Method.GET, string resource = null, IAuthenticator authToken = null);

        object Fetch();
    }
}
