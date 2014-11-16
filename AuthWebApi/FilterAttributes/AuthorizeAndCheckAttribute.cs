using AuthWebApi.Providers;
using AuthWebApi.Resources;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Web.Http;
using System.Web.Http.Controllers;

namespace AuthWebApi.FilterAttributes
{
    public class AuthorizeAndCheckAttribute : AuthorizeAttribute
    {
        public UserProvider UserProvider { get; set; }

        public AuthorizeAndCheckAttribute()
        {
            this.UserProvider = new UserProvider();
        }

        public override void OnAuthorization(HttpActionContext actionContext)
        {
            bool isOk = true;
            if (this.IsAuthorized(actionContext))
                isOk = this.CheckTimeStamps(actionContext);

            if (isOk)
                base.OnAuthorization(actionContext);
        }

        private bool CheckTimeStamps(HttpActionContext actionContext)
        {
            var identity = actionContext.RequestContext.Principal.Identity as ClaimsIdentity;
            if (this.UserProvider.IsRegisteredUserUpdated(identity))
            {
                actionContext.Response = new HttpResponseMessage(HttpStatusCode.Forbidden)
                {
                    ReasonPhrase = Exceptions.UserDataChanged
                };

                return false;
            }

            return true;
        }
    }
}