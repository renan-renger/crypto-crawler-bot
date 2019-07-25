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
        private const int satoshiToBtcRate = 100000000;

        private readonly string[] resources = {
            "getdifficulty",
            "bcperblock",
            "avgtxsize/100",
            "avgtxvalue/100",
            "hashrate",
            "interval",
            "avgtxnumber/100",
            "24hrprice",
            "24hrtransactioncount",
            "24hrbtcsent",
            "unconfirmedcount"
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
            BlockchainInfoDomain retInfo = new BlockchainInfoDomain
            {
                Timestamp = DateTime.UtcNow.AsUtc()
            };

            tmpTuple.ForEach(tuple =>
            {
                var (resource, fetchedString) = tuple;

                switch (resource)
                {
                    case "getdifficulty":
                        retInfo.CurrentDifficulty = double.Parse(fetchedString);
                        break;
                    case "bcperblock":
                        retInfo.BtcRewardPerBlock = int.Parse(fetchedString) / satoshiToBtcRate;
                        break;
                    case "avgtxsize/1000":
                        retInfo.AvgTxSizeBytes = decimal.Parse(fetchedString);
                        break;
                    case "avgtxvalue/1000":
                        retInfo.AvgTxValueUsd = double.Parse(fetchedString);
                        break;
                    case "hashrate":
                        retInfo.Hashrate = (int)(long.Parse(fetchedString) / 1000);
                        break;
                    case "interval":
                        retInfo.BlockGenerationInterval = decimal.Parse(fetchedString);
                        break;
                    case "avgtxnumber/1000":
                        retInfo.AvgTxPerBlock = (int)double.Parse(fetchedString);
                        break;
                    case "24hrprice":
                        retInfo.WeightedAvgPriceUsd = decimal.Parse(fetchedString);
                        break;
                    case "24hrtransactioncount":
                        retInfo.DailyTransactionCount = int.Parse(fetchedString);
                        break;
                    case "24hrbtcsent":
                        retInfo.DailyBtcSent = decimal.Parse(fetchedString) / satoshiToBtcRate;
                        break;
                    case "unconfirmedcount":
                        retInfo.UnconfirmedTransactionCount = int.Parse(fetchedString);
                        break;
                }
            });


            return retInfo;
        }
    }
}
