using AuthWebApi.Models.Account;
using System.Security.Claims;

namespace AuthWebApi.Providers.ClaimsMappingStrategies
{
    public class RegisteredExternal : ClaimsMapper
    {
        public RegisteredExternal(User user, ExternalLoginModel extLogin)
        {
            this.Id = user.Id.ToString();
            this.Email = user.Email;
            this.FullName = user.FullName ?? string.Empty;
            this.AvatarUrl = user.AvatarUrl;
            this.Sid = extLogin.ProviderKey;
            this.Version = this.GetVersion(user.TimeStamp);
            this.Issuer = ClaimsIdentity.DefaultIssuer;
            this.OriginalIssuer = extLogin.Provider.ToString();
        }
    }
}