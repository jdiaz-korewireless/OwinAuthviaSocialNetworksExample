using Newtonsoft.Json;

namespace AuthDomain.Models.Account
{
    public class ExternalLoginInfo
    {
        [JsonProperty("provider")]
        public ExternalLoginProvider ProviderType { get; set; }

        [JsonProperty("key", Required = Required.Always)]
        public string ProviderKey { get; set; }
    }
}
