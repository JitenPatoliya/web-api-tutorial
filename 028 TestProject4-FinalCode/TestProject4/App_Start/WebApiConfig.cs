using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using TestProject4.Handlers;

namespace TestProject4
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Web API configuration and services
            //config.MessageHandlers.Add(new FullPipelineTimerHandler());
            //config.MessageHandlers.Add(new ApiKeyHeaderHandler());
            //config.MessageHandlers.Add(new RemoveBadHeadersHandler());  // remember this one doesn't work!
            config.MessageHandlers.Add(new ForwardedHeadersHandler());


            // Web API routes
            config.MapHttpAttributeRoutes();

            //config.Routes.MapHttpRoute(
            //    name: "DefaultApi",
            //    routeTemplate: "api/{controller}/{id}",
            //    defaults: new { id = RouteParameter.Optional }
            //);
        }
    }
}
