using HandlerTemplates.Constraints;
using HandlerTemplates.ExceptionHandlers;
using HandlerTemplates.Filters;
using HandlerTemplates.Handlers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using System.Web.Http.ExceptionHandling;
using System.Web.Http.Routing;

namespace HandlerTemplates
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // see AuthenticationFilterTemplate notes for this setting
            config.SuppressHostPrincipal();

            // Web API configuration and services

            // note you must use Replace here, there can only be one global 
            // exception handler registered
            config.Services.Replace(typeof(IExceptionHandler), new GlobalExceptionHandlerTemplate());
            // you can have any number of exception loggers
            config.Services.Add(typeof(IExceptionLogger), new GlobalExceptionLoggerTemplate());

            // register our Delegating Handlers
            config.MessageHandlers.Add(new DelegatingHandlerTemplate());

            // register our Authentication, Authorization and Action filters
            // (for those we want active globally)
            config.Filters.Add(new AuthenticationFilterTemplateAttribute());
            config.Filters.Add(new AuthorizationFilterTemplateAttribute());
            config.Filters.Add(new ActionFilterTemplateAttribute());

            // register our Exception filters
            // (for those we want active globally)
            config.Filters.Add(new ExceptionFilterTemplateAttribute());

            // register our Constraints
            var constraintResolver = new DefaultInlineConstraintResolver();
            constraintResolver.ConstraintMap.Add(ParameterConstraintTemplate.DefaultConstraintName, 
                                                    typeof(ParameterConstraintTemplate));
            constraintResolver.ConstraintMap.Add(RegexBasedConstraintTemplate.DefaultConstraintName,
                                                    typeof(RegexBasedConstraintTemplate));
            // Web API routes
            config.MapHttpAttributeRoutes(constraintResolver);

            // Remove the template routing
            //config.Routes.MapHttpRoute(
            //    name: "DefaultApi",
            //    routeTemplate: "api/{controller}/{id}",
            //    defaults: new { id = RouteParameter.Optional },
            //    constraints: new { id = new RegexBasedConstraintTemplate() },
            //    handler: new DelegatingHandlerTemplate()
            //);
        }
    }
}
