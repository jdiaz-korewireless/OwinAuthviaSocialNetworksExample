using AuthWebApi.Models.Account;

namespace AuthWebApi.Providers.ClaimsMappingStrategies
{
    public class NotRegisteredExternal : ClaimsMapper
    {
        public NotRegisteredExternal(ExternalLoginModel extLogin)
        {
            this.Id = string.Empty;
            this.Email = extLogin.Email ?? string.Empty;
            this.FullName = extLogin.FullName ?? string.Empty;
            this.AvatarUrl = extLogin.AvatarUrl ?? string.Empty;
            this.Sid = extLogin.ProviderKey;
            this.Version = string.Empty;
            this.Issuer = extLogin.Provider.ToString();
            this.OriginalIssuer = this.Issuer;
        }
    }
}