using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CryptoCrawler.AzureFunc.Interfaces
{
    public interface ISlowDataFetcher
    {
        Task Fetch();
    }
}
