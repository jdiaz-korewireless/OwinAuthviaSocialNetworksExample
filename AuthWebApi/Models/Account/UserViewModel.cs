using Newtonsoft.Json;

namespace AuthWebApi.Models.Account
{
    public class UserViewModel
    {
        [JsonProperty("email", Required = Required.Always)]
        public string Email { get; set; }

        [JsonProperty("name", Required = Required.Always)]
        public string FullName { get; set; }

        [JsonProperty("ava")]
        public string AvatarUrl { get; set; }

        [JsonProperty("isReg", Required = Required.Always)]
        public bool IsRegistered { get; set; }

        [JsonProperty("verified", Required = Required.Always)]
        public bool IsVerified { get; set; }

        [JsonProperty("provider", Required = Required.Always)]
        public string LoginProvider { get; set; }
    }
}