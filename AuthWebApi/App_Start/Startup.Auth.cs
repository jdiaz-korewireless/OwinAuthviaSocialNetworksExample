using Microsoft.AspNet.Identity;
using Microsoft.Owin;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.Facebook;
using Microsoft.Owin.Security.Google;
using Microsoft.Owin.Security.MicrosoftAccount;
using Microsoft.Owin.Security.OAuth;
using Owin;
using AuthWebApi.Providers.OAuthProviders;
using AuthWebApi.Resources;
using System;

namespace AuthWebApi
{
    public partial class Startup
    {
        static Startup()
        {
            PublicClientId = "self";
            ExternalAuthPageUrl = "Content/ExtAuthRequest.htm";
            var applicationOAuthProvider = new ApplicationOAuthProvider(PublicClientId, ExternalAuthPageUrl);
            OAuthOptions = new OAuthAuthorizationServerOptions
            {
                TokenEndpointPath = new PathString("/api/token"),
                Provider = applicationOAuthProvider,
                RefreshTokenProvider = applicationOAuthProvider,
                AuthorizeEndpointPath = new PathString("/api/account/externalLogin"),
                AccessTokenExpireTimeSpan = TimeSpan.FromDays(30),
                AllowInsecureHttp = false
            };
        }

        public static OAuthAuthorizationServerOptions OAuthOptions { get; private set; }
        public static string PublicClientId { get; private set; }
        public static string ExternalAuthPageUrl { get; private set; }

        // For more information on configuring authentication, please visit http://go.microsoft.com/fwlink/?LinkId=301864
        public void ConfigureAuth(IAppBuilder app)
        {
            // Enable the application to use a cookie to store information for the signed in user
            // and to use a cookie to temporarily store information about a user logging in with a third party login provider
            app.UseCookieAuthentication(new CookieAuthenticationOptions());
            app.UseExternalSignInCookie(DefaultAuthenticationTypes.ExternalCookie);

            // Enable the application to use bearer tokens to authenticate users
            app.UseOAuthBearerTokens(OAuthOptions);

            EnableMicrosoftAuth(app);
            EnableFacebookAuth(app);
            EnableGoogleAuth(app);
        }

        private static void EnableMicrosoftAuth(IAppBuilder app)
        {
            var microSoftAuthOpt = new MicrosoftAccountAuthenticationOptions();
            microSoftAuthOpt.ClientId = Credentials.Microsoft_Id;
            microSoftAuthOpt.ClientSecret = Credentials.Microsoft_Secret;
            microSoftAuthOpt.Scope.Add("wl.emails");
            microSoftAuthOpt.Provider = new MicrosoftOAuthProvider();

            app.UseMicrosoftAccountAuthentication(microSoftAuthOpt);
        }

        private static void EnableFacebookAuth(IAppBuilder app)
        {
            var facebookAuthOpt = new FacebookAuthenticationOptions();
            facebookAuthOpt.AppId = Credentials.Facebook_Id;
            facebookAuthOpt.AppSecret = Credentials.Facebook_Secret;
            facebookAuthOpt.Scope.Add("email");
            facebookAuthOpt.Provider = new FacebookOAuthProvider();

            app.UseFacebookAuthentication(facebookAuthOpt);
        }

        private static void EnableGoogleAuth(IAppBuilder app)
        {
            var authOptions = new GoogleOAuth2AuthenticationOptions();
            authOptions.ClientId = Credentials.Google_Id;
            authOptions.ClientSecret = Credentials.Google_Secret;
            authOptions.Scope.Add("email");
            authOptions.Provider = new GoogleOAuthProvider();

            app.UseGoogleAuthentication(authOptions);
        }
    }
}