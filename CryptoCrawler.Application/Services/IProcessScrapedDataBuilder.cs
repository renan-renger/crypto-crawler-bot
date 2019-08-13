using CryptoCrawler.Contracts.Messaging.Command;
using System.Collections.Generic;

namespace CryptoCrawler.Application.Services
{
    public interface IProcessScrapedDataBuilder
    {
        ProcessScrapedData BuildCommand(IList<object> payload, string endpoint);
    }
}
