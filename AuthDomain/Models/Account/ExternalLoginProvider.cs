using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace AuthDomain.Models.Account
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum ExternalLoginProvider : byte
    {
        None = 0,

        Google = 1,

        Facebook = 2,

        Microsoft = 3
    }
}
