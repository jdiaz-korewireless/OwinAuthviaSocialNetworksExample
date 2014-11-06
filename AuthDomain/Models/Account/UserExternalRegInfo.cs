using Newtonsoft.Json;
using System.Collections.Generic;

namespace AuthDomain.Models.Account
{
    public class UserExternalRegInfo : User
    {
        [JsonProperty("extLogins")]
        public IList<ExternalLoginInfo> ExternalLoginInfo { get; set; }
    }
}
