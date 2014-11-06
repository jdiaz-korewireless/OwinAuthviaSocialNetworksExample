using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.Infrastructure;
using Microsoft.Owin.Security.OAuth;
using AuthWebApi.Models.Account;
using AuthWebApi.Providers.ClaimsMappingStrategies;
using AuthWebApi.Resources;
using System;
using System.Collections.Generic;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;

namespace AuthWebApi.Providers.OAuthProviders
{
    public class ApplicationOAuthProvider : OAuthAuthorizationServerProvider, IAuthenticationTokenProvider
    {
        private readonly string _publicClientId;
        public UserProvider UserProvider { get; set; }

        public ApplicationOAuthProvider(string publicClientId)
        {
            if (publicClientId == null)
            {
                throw new ArgumentNullException("publicClientId");
            }

            _publicClientId = publicClientId;
            this.UserProvider = new UserProvider();
        }

        public override async Task GrantResourceOwnerCredentials(OAuthGrantResourceOwnerCredentialsContext context)
        {
            User user = await this.UserProvider.FindAsync(context.UserName, context.Password);

            if (user == null)
            {
                context.SetError("invalid_grant", Exceptions.InvalidGrant);
                return;
            }

            ClaimsMapper claimsMapper = new RegisteredLocal(user);
            ClaimsIdentity oAuthIdentity = this.UserProvider.CreateIdentity(claimsMapper, context.Options.AuthenticationType);
            ClaimsIdentity cookiesIdentity = this.UserProvider.CreateIdentity(claimsMapper, CookieAuthenticationDefaults.AuthenticationType);
            AuthenticationProperties properties = CreateProperties(user);
            AuthenticationTicket ticket = new AuthenticationTicket(oAuthIdentity, properties);
            context.Validated(ticket);
            context.Request.Context.Authentication.SignIn(cookiesIdentity);
        }

        public override Task TokenEndpoint(OAuthTokenEndpointContext context)
        {
            foreach (KeyValuePair<string, string> property in context.Properties.Dictionary)
            {
                context.AdditionalResponseParameters.Add(property.Key, property.Value);
            }

            return Task.FromResult<object>(null);
        }

        public override Task ValidateClientAuthentication(OAuthValidateClientAuthenticationContext context)
        {
            // Resource owner password credentials does not provide a client ID.
            if (context.ClientId == null)
            {
                context.Validated();
            }

            return Task.FromResult<object>(null);
        }

        public override Task ValidateClientRedirectUri(OAuthValidateClientRedirectUriContext context)
        {
            if (context.ClientId == _publicClientId)
            {
                Uri expectedRootUri = new Uri(context.Request.Uri, "/");

                if (expectedRootUri.AbsoluteUri == context.RedirectUri)
                {
                    context.Validated();
                }
            }

            return Task.FromResult<object>(null);
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
                { "ava", user.AvatarUrl ?? string.Empty }
            };
            return new AuthenticationProperties(data);
        }

        public void Create(AuthenticationTokenCreateContext context)
        {
            context.SetToken(context.SerializeTicket());
        }

        public Task CreateAsync(AuthenticationTokenCreateContext context)
        {
            return Task.Factory.StartNew(() => this.Create(context));
        }

        public void Receive(AuthenticationTokenReceiveContext context)
        {
            context.DeserializeTicket(context.Token);
            var isUpdated = this.UserProvider.IsRegisteredUserUpdated(context.Ticket.Identity);

            if (isUpdated)
            {
                context.Response.StatusCode = (int)HttpStatusCode.Forbidden;
                context.Response.ReasonPhrase = Exceptions.PasswordChanged;
            }
        }

        public Task ReceiveAsync(AuthenticationTokenReceiveContext context)
        {
            return Task.Factory.StartNew(() => this.Receive(context));
        }
    }
}