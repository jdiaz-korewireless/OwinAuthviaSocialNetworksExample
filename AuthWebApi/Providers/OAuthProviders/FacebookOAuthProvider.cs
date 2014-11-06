using Microsoft.Owin.Security.Facebook;
using System.Security.Claims;
using System.Threading.Tasks;

namespace AuthWebApi.Providers.OAuthProviders
{
    public class FacebookOAuthProvider : FacebookAuthenticationProvider
    {
        private const string ApiBaseUrl = "https://graph.facebook.com";

        public override Task Authenticated(FacebookAuthenticatedContext context)
        {
            string avatarUrl = GetAvatarUrl(context.User.GetValue("id").ToString(), 240);
            context.Identity.AddClaim(
                new Claim(UserProvider.ClaimTypeAvatarUrl, avatarUrl));

            return base.Authenticated(context);
        }

        public static string GetAvatarUrl(string facebookUserId, int size)
        {
            return string.Format("{0}/{1}/picture?width={2}&height={2}",
                ApiBaseUrl,
                facebookUserId,
                size);
        }
    }
}