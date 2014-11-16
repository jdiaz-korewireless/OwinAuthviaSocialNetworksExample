using AuthDomain.Models.Account;
using AuthWebApi.Resources;
using AuthWebApi.Utils;
using Microsoft.Owin.Security.Infrastructure;
using Microsoft.Owin.Security.OAuth;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

namespace AuthWebApi.Providers.OAuthProviders
{
    public class ApplicationOAuthProvider : OAuthAuthorizationServerProvider, IAuthenticationTokenProvider
    {
        private readonly string publicClientId;
        private readonly string externalAuthPageUrl;
        public UserProvider UserProvider { get; set; }

        public ApplicationOAuthProvider(string publicClientId, string externalAuthPageUrl)
        {
            if (publicClientId == null)
            {
                throw new ArgumentNullException("publicClientId");
            }

            this.publicClientId = publicClientId;
            this.externalAuthPageUrl = externalAuthPageUrl;
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

            OwinHelper.SingIn(context, user);
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
            if (context.ClientId == publicClientId)
            {
                Uri expectedRootUri = new Uri(context.Request.Uri, "/" + this.externalAuthPageUrl);

                if (expectedRootUri.AbsoluteUri == context.RedirectUri)
                {
                    context.Validated();
                }
            }

            return Task.FromResult<object>(null);
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
                context.Response.ReasonPhrase = Exceptions.UserDataChanged;
            }
        }

        public Task ReceiveAsync(AuthenticationTokenReceiveContext context)
        {
            return Task.Factory.StartNew(() => this.Receive(context));
        }
    }
}