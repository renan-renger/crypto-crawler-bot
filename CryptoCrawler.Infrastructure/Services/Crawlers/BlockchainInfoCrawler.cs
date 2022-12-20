using System;
using System.Collections.Generic;
using System.Linq;
using CryptoCrawler.Contracts.Domain;
using CryptoCrawler.Application.Services;
using RestSharp;
using RestSharp.Authenticators;

namespace CryptoCrawler.Infrastructure.Services.Crawlers
{
    public class BlockchainInfoCrawler : IApiCrawler<BlockchainInfoDomain>
    {
        private static RestClient _restClient;
        private static RestRequest _restRequest;
        private const string endpointAddress = "https://blockchain.info/q";
        private const Method method = Method.Get;

        private readonly string[] resources = {
            "getdifficulty",
            "bcperblock",
            "avgtxsize/1000",
            "avgtxvalue/1000",
            "hashrate",
            "interval",
            "avgtxnumber/1000",
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

                _restClient.Options.BaseUrl = new Uri(endpointAddress);

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
            var tmpTuple = new List<Tuple<string, string>>();

            tmpTuple.AddRange(resources.Select(resource =>
            {
                _restRequest.Resource = resource;
                var response = _restClient.Execute(_restRequest);
                return new Tuple<string, string>(resource, response.IsSuccessful ? response.Content : string.Empty);
            }));

            return ProcessFetched(tmpTuple);
        }

        public string ExposeEndpoint() { return endpointAddress; }

        internal BlockchainInfoDomain ProcessFetched(List<Tuple<string, string>> tmpTuple)
        {
            var retInfo = new BlockchainInfoDomain();

            tmpTuple.ForEach(tuple =>
            {
                var (resource, fetchedString) = tuple;

                switch (resource)
                {
                    case "getdifficulty":
                        retInfo.CurrentDifficulty = double.TryParse(fetchedString, out var tmpCD) ? tmpCD : (double?)null;
                        break;
                    case "bcperblock":
                        retInfo.BtcRewardPerBlock = int.TryParse(fetchedString, out var tmpRPB) ? tmpRPB / 100000000 : (int?)null;
                        break;
                    case "avgtxsize/1000":
                        retInfo.AvgTxSizeBytes = decimal.TryParse(fetchedString, out var tmpTxBytes) ? tmpTxBytes : (decimal?)null;
                        break;
                    case "avgtxvalue/1000":
                        retInfo.AvgTxValueUsd = double.TryParse(fetchedString, out var tmpTxValue) ? tmpTxValue : (double?)null;
                        break;
                    case "hashrate":
                        retInfo.Hashrate = long.TryParse(fetchedString, out var tmpHashRate) ? (int)(tmpHashRate / 1000) : (int?)null;
                        break;
                    case "interval":
                        retInfo.BlockGenerationInterval = decimal.TryParse(fetchedString, out var tmpBlkGen) ? tmpBlkGen : (decimal?)null;
                        break;
                    case "avgtxnumber/1000":
                        retInfo.AvgTxPerBlock = double.TryParse(fetchedString, out var tmpTxBlc) ? (int)tmpTxBlc : (int?)null;
                        break;
                    case "24hrprice":
                        retInfo.WeightedAvgPriceUsd = decimal.TryParse(fetchedString, out var tmpWgPrice) ? tmpWgPrice : (decimal?)null;
                        break;
                    case "24hrtransactioncount":
                        retInfo.DailyTransactionCount = int.TryParse(fetchedString, out var tmpTransCount) ? tmpTransCount : (int?)null;
                        break;
                    case "24hrbtcsent":
                        retInfo.DailyBtcSent = decimal.TryParse(fetchedString, out var tmpBtcSent) ? tmpBtcSent / 100000000 : (decimal?)null;
                        break;
                    case "unconfirmedcount":
                        retInfo.UnconfirmedTransactionCount = int.TryParse(fetchedString, out var tmpUnconfirmCount) ? tmpUnconfirmCount : (int?)null;
                        break;
                }
            });


            return retInfo;
        }
    }
}
