using AuthDomain.Models.Account;
using Newtonsoft.Json;

namespace AuthWebApi.Models.Account
{
    public class ExternalLoginProviderModel
    {
        [JsonProperty("name", Required = Required.Always)]
        public ExternalLoginProvider Provider { get; set; }

        [JsonProperty("url", Required = Required.Always)]
        public string Url { get; set; }

        [JsonProperty("state", Required = Required.Always)]
        public string State { get; set; }
    }
}