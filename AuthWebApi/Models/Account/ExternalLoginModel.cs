using AuthDomain.Models.Account;
using AuthWebApi.Utils;
using Microsoft.AspNet.Identity;
using System;
using System.Security.Claims;

namespace AuthWebApi.Models.Account
{
    public class ExternalLoginModel
    {
        public string Email { get; set; }

        public string FullName { get; set; }

        public string AvatarUrl { get; set; }

        public ExternalLoginProvider Provider { get; set; }

        public string ProviderKey { get; set; }

        public bool IsRegistered { get; set; }

        public static ExternalLoginModel FromIdentity(ClaimsIdentity identity)
        {
            if (identity == null)
            {
                return null;
            }

            Claim idClaim = identity.FindFirst(ClaimTypes.NameIdentifier);

            if (!IsOk(idClaim))
                return null;

            ExternalLoginModel result = new ExternalLoginModel();
            result.IsRegistered = (idClaim.Issuer == ClaimsIdentity.DefaultIssuer);

            ExternalLoginProvider loginProvider;
            if (!Enum.TryParse<ExternalLoginProvider>(idClaim.OriginalIssuer, ignoreCase: true, result: out loginProvider))
                return null;
            result.Provider = loginProvider;

            if (identity.AuthenticationType == DefaultAuthenticationTypes.ExternalCookie)
            {
                result.ProviderKey = idClaim.Value;
                result.Email = identity.FindFirstValue(ClaimTypes.Email);
                result.FullName =
                    loginProvider == ExternalLoginProvider.Facebook ?
                    identity.FindFirstValue("urn:facebook:name") :
                    identity.FindFirstValue(ClaimTypes.Name);
            }
            else
            {
                result.ProviderKey = identity.FindFirstValue(ClaimTypes.Sid);
                result.Email = identity.FindFirstValue(ClaimTypes.Email);
                result.FullName = identity.FindFirstValue(ClaimTypes.GivenName);
            }

            result.AvatarUrl = identity.FindFirstValue(OwinHelper.ClaimTypeAvatarUrl);
            return result;
        }

        private static bool IsOk(Claim idClaim)
        {
            if (idClaim == null)
                return false;

            if (String.IsNullOrEmpty(idClaim.Issuer))
                return false;

            if (String.IsNullOrEmpty(idClaim.OriginalIssuer))
                return false;

            if (idClaim.OriginalIssuer == ClaimsIdentity.DefaultIssuer)
                return false;

            if (idClaim.Issuer != idClaim.OriginalIssuer && idClaim.Value == null)
                return false;

            return true;
        }
    }
}