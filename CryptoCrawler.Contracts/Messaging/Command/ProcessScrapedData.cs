using System;
using System.Collections.Generic;

namespace CryptoCrawler.Contracts.Messaging.Command
{
    public class ProcessScrapedData
    {
        /// <summary>
        /// UTC date and hour of pooling/crawling
        /// </summary>
        public DateTime Timestamp { get; set; }

        /// <summary>
        /// Payload type, based on Domain Contracts
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// Endpoint from which the data was fetched/scraped
        /// </summary>
        public string Endpoint { get; set; }

        /// <summary>
        /// Actual scraped/fetched dataset
        /// </summary>
        public IList<object> Payload { get; set; }
    }
}
