using System;

namespace CryptoCrawler.Contracts.Domain
{
    public class MercadoBitcoinTicker
    {
        public double? LastBuy { get; set; }
        public DateTime LastUpdate { get; set; }
        public double? HighestDaily { get; set; }
        public double? ClosePrice { get; set; }
        public double? LowestDaily { get; set; }
        public double? OpenPrice { get; set; }
        public string PairName { get; set; }
        public double? LastSell { get; set; }
        public double? TradedVolume { get; set; }

    }
}
