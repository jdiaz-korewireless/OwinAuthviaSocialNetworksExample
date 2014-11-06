using AuthDomain.Models.Account;
using System.Security.Claims;

namespace AuthWebApi.Providers.ClaimsMappingStrategies
{
    public class RegisteredLocal : ClaimsMapper
    {
        public RegisteredLocal(User user)
        {
            this.Id = user.Id.ToString();
            this.Email = user.Email;
            this.FullName = user.FullName ?? string.Empty;
            this.AvatarUrl = user.AvatarUrl;
            this.Sid = string.Empty;
            this.Version = this.GetVersion(user.TimeStamp);
            this.Issuer = ClaimsIdentity.DefaultIssuer;
            this.OriginalIssuer = ClaimsIdentity.DefaultIssuer;
        }
    }
}