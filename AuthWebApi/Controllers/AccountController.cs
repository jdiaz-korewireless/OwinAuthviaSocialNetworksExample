using AuthDomain;
using AuthDomain.Models.Account;
using AuthWebApi.HttpActionResult;
using AuthWebApi.Models;
using AuthWebApi.Models.Account;
using AuthWebApi.Providers;
using AuthWebApi.Resources;
using AuthWebApi.Utils;
using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.OAuth;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web.Http;

namespace AuthWebApi.Controllers
{
    [RoutePrefix("api/account")]
    public class AccountController : ApiControllerBase
    {
        public UserProvider UserProvider { get; set; }
        public EmailProvider EmailProvider { get; set; }

        public AccountController()
        {
            this.UserProvider = new UserProvider();
            this.EmailProvider = new EmailProvider();
        }

        // GET api/account/user
        [HostAuthentication(DefaultAuthenticationTypes.ExternalBearer)]
        [Route("user")]
        public UserViewModel GetUser()
        {
            ClaimsIdentity userIdentity = User.Identity as ClaimsIdentity;
            ExternalLoginModel externalLogin = ExternalLoginModel.FromIdentity(userIdentity);

            var user = new UserViewModel
            {
                Email = userIdentity.FindFirstValue(ClaimTypes.Email),
                FullName = userIdentity.FindFirstValue(ClaimTypes.GivenName),
                IsVerified = Boolean.Parse(userIdentity.FindFirstValue(OwinHelper.ClaimTypeIsVerified)),
                AvatarUrl = userIdentity.FindFirstValue(OwinHelper.ClaimTypeAvatarUrl),
                IsRegistered = (externalLogin == null || externalLogin.IsRegistered),
                LoginProvider = (externalLogin != null ? externalLogin.Provider.ToString() : null)
            };

            return user;
        }

        // GET api/account/avatar/123
        [AllowAnonymous]
        [Route("avatar/{userId:int}")]
        public async Task<HttpResponseMessage> GetAvatar(int userId)
        {
            byte[] avatar = await this.UserProvider.GetAvatarAsync(userId);

            var response = Request.CreateResponse(HttpStatusCode.OK, avatar, "application/octet-stream");
            response.Content.Headers.ContentType = new MediaTypeHeaderValue("image/jpg");
            response.Headers.CacheControl = new CacheControlHeaderValue
            {
                MaxAge = new TimeSpan(24, 0, 0),
                Public = true
            };

            return response;
        }

        // POST api/account/verify
        [Route("verify")]
        public async Task<NotificationResult> Verify()
        {
            var userVerification = await this.UserProvider.VerifyAsync(User.Identity);

            this.EmailProvider.SendConfirmationCodeAsync(userVerification);

            return new NotificationResult(userVerification.User.Email);
        }

        // DELETE api/account        
        [Route("user")]
        public async Task<NotificationResult> DeleteUser()
        {
            var user = OwinHelper.CreateUser(User.Identity as ClaimsIdentity);

            await this.UserProvider.DeleteUserWithDependenciesAsync(user);
            this.EmailProvider.SendDeleteConfirmationAsync(user);

            return new NotificationResult(user.Email);
        }

        #region External Login

        // GET api/Account/ExternalLogin
        [AllowAnonymous]
        [HostAuthentication(DefaultAuthenticationTypes.ExternalCookie)] //authenticated by external provider
        [HostAuthentication(DefaultAuthenticationTypes.ExternalBearer)] //refresh token support
        [Route("externalLogin", Name = "externalLogin")]
        public async Task<IHttpActionResult> GetExternalLogin(string provider, string error = null)
        {
            if (error != null)
            {
                return Redirect(Url.Content("~/") + "#error=" + Uri.EscapeDataString(error));
            }

            ExternalLoginProvider loginProvider;
            if (!Enum.TryParse<ExternalLoginProvider>(provider, ignoreCase: true, result: out loginProvider) ||
                loginProvider == ExternalLoginProvider.None)
            {
                //Unsupported login provider
                return InternalServerError();
            }

            if (!User.Identity.IsAuthenticated)
            {
                return new ChallengeResult(loginProvider, this);
            }

            ExternalLoginModel externalLogin = ExternalLoginModel.FromIdentity(User.Identity as ClaimsIdentity);

            if (externalLogin == null)
            {
                return InternalServerError();
            }

            if (externalLogin.Provider != loginProvider)
            {
                Request.GetOwinContext().Authentication.SignOut(
                    DefaultAuthenticationTypes.ExternalCookie,
                    OAuthDefaults.AuthenticationType,
                    CookieAuthenticationDefaults.AuthenticationType);
                return new ChallengeResult(loginProvider, this);
            }

            User user = await this.UserProvider.FindAsync(externalLogin.Provider, externalLogin.ProviderKey);
            if (user != null)
            {
                OwinHelper.SingIn(Request.GetOwinContext(), user, externalLogin);
            }
            else
            {
                OwinHelper.SingIn(Request.GetOwinContext(), externalLogin);
            }

            return Ok();
        }

        // GET api/account/externalLogins?returnUrl=%2F&generateState=true
        [AllowAnonymous]
        [Route("externalLogins")]
        public IList<ExternalLoginProviderModel> GetExternalLogins(string returnUrl, bool generateState = false)
        {
            IEnumerable<AuthenticationDescription> descriptions = Request.GetOwinContext().Authentication.GetExternalAuthenticationTypes();
            IList<ExternalLoginProviderModel> providers = new List<ExternalLoginProviderModel>();

            string state;

            if (generateState)
            {
                const int strengthInBits = 256;
                state = RandomOAuthStateGenerator.Generate(strengthInBits);
            }
            else
            {
                state = null;
            }

            foreach (AuthenticationDescription description in descriptions)
            {
                var loginProvider = (ExternalLoginProvider)Enum.Parse(typeof(ExternalLoginProvider),
                    description.Caption,
                    ignoreCase: true);

                ExternalLoginProviderModel model = new ExternalLoginProviderModel
                {
                    Provider = loginProvider,
                    Url = Url.Route("externalLogin", new
                    {
                        provider = description.AuthenticationType,
                        response_type = "token",
                        client_id = Startup.PublicClientId,
                        redirect_uri = new Uri(Request.RequestUri, returnUrl).AbsoluteUri + Startup.ExternalAuthPageUrl,
                        state = state
                    }),
                    State = state
                };

                providers.Add(model);
            }

            return providers;
        }

        // POST api/account/registerExternal
        [OverrideAuthentication]
        [HostAuthentication(DefaultAuthenticationTypes.ExternalBearer)]
        [Route("registerExternal")]
        public async Task<RegistrationResult> RegisterExternal()
        {
            ExternalLoginModel externalLogin = ExternalLoginModel.FromIdentity(User.Identity as ClaimsIdentity);

            if (externalLogin == null)
            {
                throw new ApiException(Exceptions.ExternalLoginNotFound);
            }

            var user = await this.UserProvider.CreateExternalAsync(externalLogin);
            var userViewModel = UserProvider.MapUserToViewModel(user, externalLogin);

            OwinHelper.SingIn(Request.GetOwinContext(), user, externalLogin);
            var token = OwinHelper.CreateToken(Request.GetOwinContext(), user, externalLogin);

            return new RegistrationResult(userViewModel, token);
        }

        #endregion
	}
}