using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Principal;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;

namespace HandlerTemplates.Filters
{
    /// <summary>
    /// Authorization happens AFTER authentication. By the time your authorization filter
    /// is called the authenticated identity should be set (if credentials were provided).
    /// </summary>
    // TODO: Decide if your filter should allow multiple instances per controller or
    //       per-method; set AllowMultiple to false if not
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
    public class AuthorizationFilterTemplateAttribute : AuthorizationFilterAttribute
    {
        // TODO: If you need constructor arguments, create properties to hold them
        //       and public constructors that accept them.
        public AuthorizationFilterTemplateAttribute()
        { }

        /// <summary>
        /// Called when authorization must be checked; 
        /// </summary>
        public override async Task OnAuthorizationAsync(HttpActionContext actionContext, 
            CancellationToken cancellationToken)
        {
            // STEP 1: Perform your authorization logic
            // The authentication filters should have set an IPrincipal for you 
            // with various properties
            var principal = actionContext.RequestContext.Principal;

            //...though it is possible to have an authorization filter without or 
            // independent of authentication; perhaps based the presence of certain 
            // http headers in the request.  In that case use the appropriate logic. 

            // You can cast the IPrincipal to a specific class type to access the 
            // claims or properties of the authenticated principal:
            //var specificIdentityType = principal.Identity as ClaimsIdentity;
            //var claim = specificIdentityType.Claims.FirstOrDefault(a => a.Type.Equals("MyClaim"));

            var authorized = true; // DoSomeAuthorizationLogicMaybeEvenAsync();

            // STEP 2: If authorization fails, set the HTTP reponse and exit
            if (!authorized)
            {
                // Which code to return is a bit religious. https://stackoverflow.com/questions/3297048/403-forbidden-vs-401-unauthorized-http-responses
                // But I prefer either 403 Forbidden or 404 Not Found for authorization issues 
                // (404 if for security reasons you want to disguise the fact that it was 
                // an authorization issue, to avoid giving an attacker too much information).
                actionContext.Response = new HttpResponseMessage(HttpStatusCode.Forbidden)
                ;//{ Content = ...};
                return;
            }

            await base.OnAuthorizationAsync(actionContext, cancellationToken);
        }

    }
}