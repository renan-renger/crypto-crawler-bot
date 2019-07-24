using System;

namespace CryptoCrawler.Application.Domain
{
    public class BlockchainInfoDomain
    {
        public DateTime Timestamp { get; set; }

        /// <summary>
        /// Decimal representation for current difficulty of mining process - Smaller numbers means easier
        /// </summary>
        public double CurrentDifficulty { get; set; }

        /// <summary>
        /// Current reward (in BTC) per block mined
        /// </summary>
        public decimal RewardPerBlock { get; set; }

        /// <summary>
        /// Average transaction size, in Bytes. Based on the last 1000 transactions
        /// </summary>
        public decimal AvgTxSizeBytes { get; set; }

        /// <summary>
        /// Average transaction value, in USD. Based on the last 1000 transactions
        /// </summary>
        public decimal AvgTxValueUsd { get; set; }
    }
}
