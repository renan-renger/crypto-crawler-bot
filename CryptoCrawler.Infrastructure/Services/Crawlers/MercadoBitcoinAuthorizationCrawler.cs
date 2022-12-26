using CryptoCrawler.Application.Services;
using CryptoCrawler.InternalContracts.Domain;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;
using RestSharp.Authenticators;
using System;
using System.Linq;

namespace CryptoCrawler.Infrastructure.Services.Crawlers
{
    public class MercadoBitcoinAuthorizationCrawler : IApiCrawler<MercadoBitcoinAuthorizationResponse>
    {
        private static RestClient _restClient;
        private static RestRequest _restRequest;
        private static IConfiguration _configuration;
        private static MercadoBitcoinAuthorizationRequest _authCredentials;
        private const string endpointAddress = "https://api.mercadobitcoin.net/api/v4";
        private const Method method = Method.Post;

        private readonly string[] resources = {
            "authorize"
        };

        public MercadoBitcoinAuthorizationCrawler(IConfiguration configuration) {
            _restClient = new RestClient();
            _restRequest = new RestRequest();
            _authCredentials = new MercadoBitcoinAuthorizationRequest();
            _configuration = configuration;            
        }

        public string ExposeEndpoint()
        {
            return endpointAddress;
        }

        public MercadoBitcoinAuthorizationResponse Fetch()
        {
            var authData = new MercadoBitcoinAuthorizationResponse();

            _restRequest.Resource = resources.First();
            _restRequest.AddBody(JsonConvert.SerializeObject(_authCredentials));

            var response = _restClient.Execute(_restRequest);
            if (response.IsSuccessful)
            {
                var payload = JObject.Parse(response.Content);
                authData.AccessToken = payload["access_token"].ToString();
                authData.Expiration = DateTimeOffset.FromUnixTimeSeconds(long.Parse(payload["expiration"].Value<string>())).DateTime;
            }

            return authData;
        }

        public bool SetupCrawler(IAuthenticator authToken = null)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(endpointAddress))
                    return false;

                _restClient.Options.BaseUrl = new Uri(endpointAddress);
               
                _restRequest.Method = method;

                if (string.IsNullOrWhiteSpace(_configuration["MercadoBitcoinApiUser"]) || string.IsNullOrWhiteSpace(_configuration["MercadoBitcoinApiPassword"]))
                    return false;

                _authCredentials.UserId = _configuration["MercadoBitcoinApiUser"];
                _authCredentials.Password = _configuration["MercadoBitcoinApiPassword"];

                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
