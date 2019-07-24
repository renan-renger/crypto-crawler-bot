using System;
using System.Collections.Generic;
using System.Linq;
using CryptoCrawler.Application.Domain;
using CryptoCrawler.Application.Services;
using CryptoCrawler.Infrastructure.Extensions;
using RestSharp;
using RestSharp.Authenticators;

namespace CryptoCrawler.Infrastructure.Services.Crawlers
{
    public class BlockchainInfoCrawler : IApiCrawler<BlockchainInfoDomain>
    {
        private static IRestClient _restClient;
        private static IRestRequest _restRequest;
        private const string endpointAddress = "https://blockchain.info/q";
        private const Method method = Method.GET;

        private readonly string[] resources = {
            "hashrate",
            "getdifficulty"
        };

        public BlockchainInfoCrawler()
        {
            _restClient = new RestClient();
            _restRequest = new RestRequest();
        }

        public bool SetupCrawler(IAuthenticator authToken = null)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(endpointAddress))
                    return false;

                _restClient.BaseUrl = new Uri(endpointAddress);

                if (authToken != null)
                    _restClient.Authenticator = authToken;

                _restRequest.Method = method;

                return true;
            }
            catch
            {
                return false;
            }
        }

        public BlockchainInfoDomain Fetch()
        {
            List<Tuple<string,string>> tmpTuple = new List<Tuple<string, string>>();

            tmpTuple.AddRange(resources.Select(resource =>
            {
                _restRequest.Resource = resource;
                return new Tuple<string, string>(resource, _restClient.Execute(_restRequest).Content);
            }));

            return ProcessFetched(tmpTuple);
        }

        internal BlockchainInfoDomain ProcessFetched(List<Tuple<string, string>> tmpTuple)
        {
            BlockchainInfoDomain retInfo = new BlockchainInfoDomain();

            tmpTuple.ForEach(tuple =>
            {
                switch (tuple.Item1)
                {
                    case "hashrate":

                        break;
                }
            });


            return retInfo;
        }
    }
}
