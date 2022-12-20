using Newtonsoft.Json;

namespace CryptoCrawler.InternalContracts.Domain
{
    public class MercadoBitcoinAuthorizationRequest
    {
        [JsonProperty("login")]
        public string UserId { get; set; }

        [JsonProperty("password")]
        public string Password { get; set; }
    }
}
