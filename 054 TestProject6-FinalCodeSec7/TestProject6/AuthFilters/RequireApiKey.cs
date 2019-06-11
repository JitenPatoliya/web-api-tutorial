using TestProject6.Handlers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;


namespace TestProject6.AuthFilters
{
    /// <summary>
    ///  Section 7 Excercise solution - AuthZ filter to require api key of a type
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false)]
    public class RequireApiKey : AuthorizationFilterAttribute
    {
        /// <summary>
        /// Hold the type of key the action method requires
        /// </summary>
        public string ApiKeyType;

        /// <summary>
        /// Constructor taking optional action method key type
        /// </summary>
        public RequireApiKey(string sType = null)
        {
            if (sType == "R" || sType == "W" || sType == null)
                ApiKeyType = sType;
            else
                throw new ArgumentException("Api key type must be R, W or null", "sType");
        }

        /// <summary>
        /// Validate the api key according to the rules
        /// </summary>
        public static bool IsValidApiKey(string sKey)
        {
            return (!String.IsNullOrEmpty(sKey) &&
                (sKey.StartsWith("R") || sKey.StartsWith("W")) &&
                sKey.Length == 6);
        }
        /// <summary>
        /// Called when authorization must be checked; verify the api key is present and 
        /// matches the action method requirement of key type
        /// </summary>
        public override Task OnAuthorizationAsync(HttpActionContext actionContext,
            CancellationToken cancellationToken)
        {
            // grab the api key the delegating handler may have stored for us to use using the extension method
            var sKey = actionContext.Request.GetApiKey();

            // validate the key - check for missing or invalid key
            if (!IsValidApiKey(sKey))
            {
                // return a 403 Forbidden, since the key was not valid
                actionContext.Response = new HttpResponseMessage(HttpStatusCode.Forbidden)
                {
                    ReasonPhrase = "Invalid API key"
                };
                return Task.FromResult(0);

            }

            // we only need to check for a specific key type if one was required
            if (!String.IsNullOrEmpty(ApiKeyType))
            {
                // check for the specific key type; really only need to check for W action using W key,
                //  since R actions work with any key, R or W.
                if ((ApiKeyType.Equals("W") && !sKey.StartsWith("W")))
                {
                    // return a 403 Forbidden, since the key was not valid for the action
                    actionContext.Response = new HttpResponseMessage(HttpStatusCode.Forbidden)
                    {
                        ReasonPhrase = "Invalid API key"
                    };
                    return Task.FromResult(0);
                }
            }

            return base.OnAuthorizationAsync(actionContext, cancellationToken);
        }
    }
}