using System;

namespace CryptoCrawler.Application.Domain
{
    public class BlockchainInfoDomain
    {
        /// <summary>
        /// UTC date and hour of pooling/crawling
        /// </summary>
        public DateTime Timestamp { get; set; }

        /// <summary>
        /// Decimal representation for current difficulty of mining process - Smaller numbers means easier
        /// </summary>
        public double CurrentDifficulty { get; set; }

        /// <summary>
        /// Current reward (in BTC) per block mined
        /// </summary>
        public int BtcRewardPerBlock { get; set; }

        /// <summary>
        /// Average transaction size, in Bytes. Based on the last 100 blocks
        /// </summary>
        public decimal AvgTxSizeBytes { get; set; }

        /// <summary>
        /// Average transaction value, in USD. Based on the last 100 blocks
        /// </summary>
        public double AvgTxValueUsd { get; set; }

        /// <summary>
        /// Bitcoin' current Hashrate, in TH/s (Terahashes per second)
        /// </summary>
        public int Hashrate { get; set; }

        /// <summary>
        /// Average time (in seconds) between blocks
        /// </summary>
        public decimal BlockGenerationInterval { get; set; }

        /// <summary>
        /// Average number of transactions per block. Based on the last 100 blocks
        /// </summary>
        public int AvgTxPerBlock { get; set; }

        /// <summary>
        /// Weighted average price from the largest exchanges over the course of the last 24 hours 
        /// </summary>
        public decimal WeightedAvgPriceUsd { get; set; }

        /// <summary>
        /// Number of transactions processed by the network in the last 24 hours
        /// </summary>
        public int DailyTransactionCount { get; set; }

        /// <summary>
        /// Number of BTC sent in the last 24 hours, in BTC
        /// </summary>
        public decimal DailyBtcSent { get; set; }

        /// <summary>
        /// Number of unconfirmed/pending transactions in the network
        /// </summary>
        public int UnconfirmedTransactionCount { get; set; }
    }
}
