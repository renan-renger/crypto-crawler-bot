using CryptoCrawler.Application.Services;
using CryptoCrawler.Contracts.Messaging.Command;
using CryptoCrawler.Infrastructure.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CryptoCrawler.Infrastructure.Services.Builders
{
    public class ProcessScrapedDataBuilder : IProcessScrapedDataBuilder
    {
        //Timestamp = DateTime.UtcNow.AsUtc()
        public ProcessScrapedData BuildCommand(IList<object> payload, string endpoint)
        {
            return new ProcessScrapedData
            {
                Endpoint = endpoint,
                Payload = payload,
                Timestamp = DateTime.UtcNow.AsUtc(),
                Type = payload.FirstOrDefault().GetType().ToString()
            };
        }
    }
}
