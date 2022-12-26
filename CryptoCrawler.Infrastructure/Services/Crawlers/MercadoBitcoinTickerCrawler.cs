using CryptoCrawler.Application.Services;
using CryptoCrawler.Contracts.Domain;
using Newtonsoft.Json.Linq;
using RestSharp;
using RestSharp.Authenticators;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace CryptoCrawler.Infrastructure.Services.Crawlers
{
    public class MercadoBitcoinTickerCrawler : IApiCrawler<List<MercadoBitcoinTicker>>
    {
        private static RestClient _restClient;
        private static RestRequest _restRequest;
        private const string endpointAddress = "https://api.mercadobitcoin.net/api/v4";
        private const Method method = Method.Get;

        private readonly string[] resources = {
            "tickers?symbols=BTC-BRL",
            "tickers?symbols=ETH-BRL",
            "tickers?symbols=BTC-ETH"
        };

        public MercadoBitcoinTickerCrawler()
        {
            _restClient = new RestClient();
            _restRequest = new RestRequest();
        }

        public string ExposeEndpoint()
        {
            return endpointAddress;
        }

        public List<MercadoBitcoinTicker> Fetch()
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

        internal List<MercadoBitcoinTicker> ProcessFetched(List<Tuple<string, string>> tmpTuple)
        {
            var retInfo = new List<MercadoBitcoinTicker>();

            tmpTuple.ForEach(tuple =>
            {
                var (resource, fetchedString) = tuple;

                var payload = JArray.Parse(fetchedString).First;

                retInfo.Add(new MercadoBitcoinTicker()
                {
                    PairName = payload["pair"].Value<string>(),
                    LastBuy = double.TryParse(payload["buy"].Value<string>(), NumberStyles.Any, CultureInfo.InvariantCulture, out var tmpLB) ? tmpLB : null,
                    LastSell = double.TryParse(payload["sell"].Value<string>(), NumberStyles.Any, CultureInfo.InvariantCulture, out var tmpLS) ? tmpLS : null,
                    OpenPrice = double.TryParse(payload["open"].Value<string>(), NumberStyles.Any, CultureInfo.InvariantCulture, out var tmpOP) ? tmpOP : null,
                    ClosePrice = double.TryParse(payload["last"].Value<string>(), NumberStyles.Any, CultureInfo.InvariantCulture, out var tmpCP) ? tmpCP : null,
                    HighestDaily = double.TryParse(payload["high"].Value<string>(), NumberStyles.Any, CultureInfo.InvariantCulture, out var tmpHD) ? tmpHD : null,
                    LowestDaily = double.TryParse(payload["low"].Value<string>(), NumberStyles.Any, CultureInfo.InvariantCulture, out var tmpLD) ? tmpLD : null,
                    TradedVolume = double.TryParse(payload["vol"].Value<string>(), NumberStyles.Any, CultureInfo.InvariantCulture, out var tmpTV) ? tmpTV : null,
                    LastUpdate = DateTimeOffset.FromUnixTimeSeconds(long.Parse(payload["date"].Value<string>())).DateTime.ToLocalTime()

                });
            });

            return retInfo;
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
    }
}
