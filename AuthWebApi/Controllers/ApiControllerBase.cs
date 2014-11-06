using RazorEngine;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;

namespace AuthWebApi.Controllers
{
    public abstract class ApiControllerBase : ApiController
    {
        public HttpResponseMessage View(string controllerName, string viewName, object model)
        {
            string viewPath = HttpContext.Current.Server.MapPath(
                string.Format("~/Views/{0}/{1}.cshtml",
                controllerName,
                viewName));

            string template = File.ReadAllText(viewPath);
            string responseContent = Razor.Parse(template, model);

            var response = new HttpResponseMessage(HttpStatusCode.OK);
            response.Content = new StringContent(responseContent, System.Text.Encoding.UTF8, "text/html");

            return response;
        }
    }
}