using Microsoft.Owin.Security.MicrosoftAccount;
using System.Security.Claims;
using System.Threading.Tasks;

namespace AuthWebApi.Providers.OAuthProviders
{
    public class MicrosoftOAuthProvider : MicrosoftAccountAuthenticationProvider
    {
        public override void ApplyRedirect(MicrosoftAccountApplyRedirectContext context)
        {
            context = new MicrosoftAccountApplyRedirectContext(
                context.OwinContext,
                context.Options,
                context.Properties,
                context.RedirectUri + "&display=touch"); //Mobile devices support

            base.ApplyRedirect(context);
        }

        public override Task Authenticated(MicrosoftAccountAuthenticatedContext context)
        {
            string avatarUrl = string.Format("https://apis.live.net/v5.0/{0}/picture",
                            context.User.GetValue("id").ToString());

            context.Identity.AddClaim(
                new Claim(UserProvider.ClaimTypeAvatarUrl, avatarUrl));

            return base.Authenticated(context);
        }
    }
}