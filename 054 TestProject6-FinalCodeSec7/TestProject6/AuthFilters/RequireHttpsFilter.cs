using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using TestProject6.Handlers;

namespace TestProject6.AuthFilters
{
    /// <summary>
    /// Authorization filter to require the client have used HTTPS protocol. 
    /// By working in conjunction with ForwardedHeadersHandler, works for load balancer
    /// scenarios too.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false)]
    public class RequireHttpsAttribute : AuthorizationFilterAttribute
    {
        /// <summary>
        /// Called when authorization must be checked; 
        /// </summary>
        public override Task OnAuthorizationAsync(HttpActionContext actionContext,
            CancellationToken cancellationToken)
        {
            // get the base url that the client actually used, taking into account load balancers
            var uri = actionContext.Request.GetSelfReferenceBaseUrl();

            if (uri == null || !uri.Scheme.ToLowerInvariant().Equals("https"))
            {
                actionContext.Response = new HttpResponseMessage(HttpStatusCode.Forbidden)
                {
                    ReasonPhrase = "HTTPS Required"
                };
                return Task.FromResult(0);
            }

            return base.OnAuthorizationAsync(actionContext, cancellationToken);
        }
    }
}