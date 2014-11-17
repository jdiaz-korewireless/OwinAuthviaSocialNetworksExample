using AuthWebApi.FilterAttributes;
using AuthWebApi.MediaTypeFormatters;
using AuthWebApi.Utils;
using Microsoft.Owin.Security.OAuth;
using System.Linq;
using System.Web.Http;

namespace AuthWebApi
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Configure Web API to use only bearer token authentication.
            //http://blogs.msdn.com/b/webdev/archive/2013/09/20/understanding-security-features-in-spa-template.aspx
            config.SuppressDefaultHostAuthentication();
            config.Filters.Add(new HostAuthenticationFilter(OAuthDefaults.AuthenticationType));

            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "ActionApi",
                routeTemplate: "api/{controller}/{action}"
            );

            config.MessageHandlers.Add(new CompressHandler());

            var appXmlType = config.Formatters.XmlFormatter.SupportedMediaTypes.FirstOrDefault(t => t.MediaType == "application/xml");
            config.Formatters.XmlFormatter.SupportedMediaTypes.Remove(appXmlType);

            //MediaType Formatters
            config.Formatters.Add(new BinaryMediaTypeFormatter());

            //Filter Attribtes
            config.Filters.Add(new ApiExceptionFilterAttribute());
            config.Filters.Add(new AuthorizeAndCheckAttribute());

            //Razor Engine layout support
            RazorLayouts.AddMainLayout();
        }
    }
}
