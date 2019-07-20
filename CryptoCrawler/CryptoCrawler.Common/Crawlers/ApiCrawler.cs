using System;
using System.Collections.Generic;
using System.Text;
using CryptoCrawler.Common.Interfaces;
using RestSharp;
using RestSharp.Authenticators;

namespace CryptoCrawler.Common.Crawlers
{
    public class ApiCrawler : IApiCrawler
    {
        private static Method _fetchMethod;
        private static string _resource;
        private static IRestClient _restClient;
        private static IRestRequest _restRequest;

        public ApiCrawler()
        {
            _restClient = new RestClient();
            _restRequest = new RestRequest();
        }

        public bool SetupCrawler(string endpointAddress, Method method = Method.GET, string resource = null, IAuthenticator authToken = null)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(endpointAddress))
                    return false;

                _restClient.BaseUrl = new Uri(endpointAddress);

                if (authToken != null)
                    _restClient.Authenticator = authToken;

                _fetchMethod = method;

                if (!string.IsNullOrWhiteSpace(resource))
                    _resource = resource;

                _restRequest.Method = _fetchMethod;

                if (!string.IsNullOrWhiteSpace(_resource))
                    _restRequest.Resource = _resource;

                return true;
            }
            catch
            {
                return false;
            }
        }

        public object Fetch()
        {
            return _restClient.Execute(_restRequest).Content;
        }
    }
}
