using AuthDomain;
using AuthWebApi.Controllers;
using AuthWebApi.Models;
using AuthWebApi.Resources;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;

namespace AuthWebApi.FilterAttributes
{
    public class ApiExceptionFilterAttribute : ExceptionFilterAttribute
    {
        public override void OnException(HttpActionExecutedContext context)
        {
            string errorMessage = (context.Exception is ApiException) ?
                context.Exception.Message :
                HandleInternalErrorAsync(context);

            var httpResponse = TryGetCustomErrorView(context.ActionContext, errorMessage);
            if (httpResponse == null)
            {
                var errorResponse = new FailureApiResult(errorMessage);
                string errorResponseJson = JsonConvert.SerializeObject(errorResponse);

                httpResponse = new HttpResponseMessage(HttpStatusCode.OK);
                httpResponse.Content = new StringContent(errorResponseJson, Encoding.UTF8, "application/json");
            }

            httpResponse.RequestMessage = context.Request;
            context.Response = httpResponse;
        }

        private static string HandleInternalErrorAsync(HttpActionExecutedContext context)
        {
            //Custom error logging can be implemented here

            int serverErrorId = 0;
            return string.Format(Exceptions.InternalServerError, serverErrorId);
        }

        private static HttpResponseMessage TryGetCustomErrorView(HttpActionContext actionContext, string errorMessage)
        {
            var viewOnError = actionContext.ActionDescriptor
                .GetCustomAttributes<ShowViewOnErrorAttribute>().SingleOrDefault();

            if (viewOnError == null)
                return null;

            var apiController = (actionContext.ControllerContext.Controller as ApiControllerBase);
            if (apiController == null)
                return null;

            return apiController.View(
                viewOnError.ControllerName,
                viewOnError.ViewName,
                errorMessage);
        }
    }
}