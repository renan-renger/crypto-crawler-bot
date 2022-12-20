using System;

namespace CryptoCrawler.InternalContracts.Domain
{
    public class MercadoBitcoinAuthorizationResponse
    {
        public string AccessToken { get; set; }
        public DateTime Expiration { get; set; }
    }
}
