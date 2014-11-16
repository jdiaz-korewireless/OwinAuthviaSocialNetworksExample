using AuthDomain.Models.Account;
using AuthWebApi.Models.Account;
using AuthWebApi.Providers;
using AuthWebApi.Providers.ClaimsMappingStrategies;
using AuthWebApi.Providers.OAuthProviders;
using Microsoft.AspNet.Identity;
using Microsoft.Owin;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.Infrastructure;
using Microsoft.Owin.Security.OAuth;
using System;
using System.Collections.Generic;
using System.Security.Claims;

namespace AuthWebApi.Utils
{
    public class OwinHelper
    {
        public static string ClaimTypeAvatarUrl = "avatarUrl";
        public static string ClaimTypeIsVerified = "isVerified";

        public static void SingIn(OAuthGrantResourceOwnerCredentialsContext context, User user)
        {
            ClaimsMapper claimsMapper = new RegisteredLocal(user);
            ClaimsIdentity oAuthIdentity = CreateIdentity(claimsMapper, context.Options.AuthenticationType);
            ClaimsIdentity cookiesIdentity = CreateIdentity(claimsMapper, CookieAuthenticationDefaults.AuthenticationType);
            AuthenticationProperties properties = CreateProperties(user);
            AuthenticationTicket ticket = new AuthenticationTicket(oAuthIdentity, properties);
            context.Validated(ticket);
            context.Request.Context.Authentication.SignIn(cookiesIdentity);
        }

        public static void SingIn(IOwinContext owinContext, ExternalLoginModel externalLogin)
        {
            var claimsMapper = new NotRegisteredExternal(externalLogin);
            var identity = CreateIdentity(claimsMapper, OAuthDefaults.AuthenticationType);
            owinContext.Authentication.SignIn(identity);
        }

        public static void SingIn(IOwinContext owinContext, User user, ExternalLoginModel externalLogin)
        {
            owinContext.Authentication.SignOut(DefaultAuthenticationTypes.ExternalCookie);
            var claimsMapper = new RegisteredExternal(user, externalLogin);
            var oAuthIdentity = CreateIdentity(claimsMapper, OAuthDefaults.AuthenticationType);
            var cookieIdentity = CreateIdentity(claimsMapper, CookieAuthenticationDefaults.AuthenticationType);
            var properties = CreateProperties(user);
            owinContext.Authentication.SignIn(properties, oAuthIdentity, cookieIdentity);
        }

        public static AccessToken CreateToken(IOwinContext owinContext, User user, ExternalLoginModel externalLogin)
        {
            var claimsMapper = new RegisteredExternal(user, externalLogin);
            return CreateToken(owinContext, user, claimsMapper);
        }

        private static AccessToken CreateToken(IOwinContext owinContext, User user, ClaimsMapper claimsMapper)
        {
            var identity = CreateIdentity(claimsMapper, OAuthDefaults.AuthenticationType);
            var ticket = new AuthenticationTicket(identity, CreateProperties(user));
            var context = new AuthenticationTokenCreateContext(owinContext,
                Startup.OAuthOptions.AccessTokenFormat,
                ticket);

            return new AccessToken
            {
                Type = OAuthDefaults.AuthenticationType,
                Value = context.SerializeTicket()
            };
        }

        public static User CreateUser(ClaimsIdentity claimsIdentity)
        {
            return new User
            {
                Id = Int32.Parse(claimsIdentity.FindFirstValue(ClaimTypes.NameIdentifier)),
                Email = claimsIdentity.FindFirstValue(ClaimTypes.Email),
                FullName = claimsIdentity.FindFirstValue(ClaimTypes.GivenName),
                IsVerified = Boolean.Parse(claimsIdentity.FindFirstValue(ClaimTypeIsVerified)),
                TimeStamp = ClaimsMapper.GetTimeStamp(claimsIdentity.FindFirstValue(ClaimTypes.Version))
            };
        }

        private static ClaimsIdentity CreateIdentity(ClaimsMapper claimsMapper, string authenticationType)
        {
            IList<Claim> claims = new List<Claim>();

            claims.Add(new Claim(ClaimTypes.NameIdentifier, claimsMapper.Id, null, claimsMapper.Issuer, claimsMapper.OriginalIssuer));
            claims.Add(new Claim(ClaimTypes.Email, claimsMapper.Email, null, claimsMapper.Issuer, claimsMapper.OriginalIssuer));
            claims.Add(new Claim(ClaimTypes.GivenName, claimsMapper.FullName, null, claimsMapper.Issuer, claimsMapper.OriginalIssuer));
            claims.Add(new Claim(ClaimTypes.Sid, claimsMapper.Sid, null, claimsMapper.Issuer, claimsMapper.OriginalIssuer));
            claims.Add(new Claim(ClaimTypes.Version, claimsMapper.Version, null, claimsMapper.Issuer, claimsMapper.OriginalIssuer));
            claims.Add(new Claim(ClaimTypeIsVerified, claimsMapper.IsVerified, null, claimsMapper.Issuer, claimsMapper.OriginalIssuer));
            claims.Add(new Claim(ClaimTypeAvatarUrl, claimsMapper.AvatarUrl, null, claimsMapper.Issuer, claimsMapper.OriginalIssuer));

            return new ClaimsIdentity(claims, authenticationType);
        }

        /// <summary>
        /// Custom properties that will be returned to the client with an access_token
        /// </summary>
        /// <param name="user">User base info</param>
        /// <returns>Properties</returns>
        public static AuthenticationProperties CreateProperties(User user)
        {
            IDictionary<string, string> data = new Dictionary<string, string>
            {
                { "email", user.Email },
                { "name", user.FullName ?? string.Empty },
                { "ava", UserProvider.GetAvatarUrl(user) ?? string.Empty }
            };
            return new AuthenticationProperties(data);
        }
    }
}