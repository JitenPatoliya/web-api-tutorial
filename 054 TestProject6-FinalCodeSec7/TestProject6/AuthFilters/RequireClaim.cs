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
    /// Authorization filter to require the IIdentity be a ClaimsIdentity  
    /// containing zero or more specific claim types.  If no claim types are in the list, then
    /// accepts any ClaimsIdentity (i.e. works like AuthorizeAttribute by requiring an
    /// authenticated IPrincipal/IIdentity where the IIdentity is a ClaimsIdentity or derived
    /// from it).
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false)]
    public class RequireClaimAttribute : AuthorizationFilterAttribute
    {
        /// <summary>
        /// List of supported token schemes
        /// </summary>
        public readonly string[] ClaimTypes = null;

        /// <summary>
        /// If true, full list of missing claims is included in the error response
        /// </summary>
        public bool IncludeMissingInResponse { get; set; }

        /// <summary>
        /// Constructor taking a single comma-delimited list of claim types,
        /// ex. [RequireClaim("urn:Issuer,urn:MyCustomClaim")]
        /// </summary>
        public RequireClaimAttribute(string sList)
        {
            if (sList != null)
                ClaimTypes = sList.Split(',')
                    .Where(a => !String.IsNullOrEmpty(a))
                    .ToArray();
        }

        /// <summary>
        /// Constructor taking a param list of claim types, 
        /// ex. [RequireClaim("urn:Issuer", "urn:MyCustomClaim", ClaimTypes.Email)]
        /// </summary>
        public RequireClaimAttribute(params string[] list)
        {
            ClaimTypes = list.Where(a => !String.IsNullOrEmpty(a))
                             .ToArray();
        }

        /// <summary>
        /// Called when authorization must be checked; verify the claim is present and not empty
        /// </summary>
        public override Task OnAuthorizationAsync(HttpActionContext actionContext,
            CancellationToken cancellationToken)
        {
            // first, ensure an IPrincipal was set, that is has an IIdentity, and that the 
            // identity was authenticated; repeats some of Authorize attribute but
            // necessary check for this attribute as well. Likely no token was present at all.
            if (actionContext.RequestContext.Principal == null ||
                actionContext.RequestContext.Principal.Identity == null ||
                !actionContext.RequestContext.Principal.Identity.IsAuthenticated)
            {
                // in this specific case, probably no token present, we want the authentication challenge 
                // code to fire so we need to return a 401 Unauthorized,  not 403 Forbidden
                // (i.e. the basic issue is lack of authentication, not authenticated but disallowed)
                actionContext.Response = new HttpResponseMessage(HttpStatusCode.Unauthorized)
                {
                    ReasonPhrase = "Unauthorized"
                };
                return Task.FromResult(0);
            }

            // user was authenticated, verify the IIdentity is a ClaimsIdentity or some derived variant
            if (!(actionContext.RequestContext.Principal.Identity is ClaimsIdentity))
            {
                // here we can return a 403 Forbidden, since the issue really is an authorization problem
                actionContext.Response = new HttpResponseMessage(HttpStatusCode.Forbidden)
                {
                    ReasonPhrase = "Incorrect token for this operation"
                };
                return Task.FromResult(0);
            }

            // if no ClaimType list given, behave similar to Authorize -- just require any ClaimsIdentity at all,
            // which the above test has already done.
            if (ClaimTypes == null || ClaimTypes.Length == 0)
            {
                return base.OnAuthorizationAsync(actionContext, cancellationToken);
            }

            // otherwise, let's search the identity's claims list for the full list of claims I need
            var claimsId = actionContext.RequestContext.Principal.Identity as ClaimsIdentity;
            var missing = new List<string>();

            foreach (var c in ClaimTypes)
            {
                if (!claimsId.HasClaim(a => a.Type.Equals(c, StringComparison.InvariantCultureIgnoreCase)))
                {
                    missing.Add(c);
                }
            }

            if (missing.Count > 0)
            {
                // return a 403 Forbidden, since the issue really is an authorization problem;
                // get content-negotiated list if IncludeMissingInResponse is true
                if (IncludeMissingInResponse)
                    actionContext.Response = actionContext.Request.CreateResponse(HttpStatusCode.Forbidden,
                        missing);
                else
                    actionContext.Response = actionContext.Request.CreateResponse(HttpStatusCode.Forbidden);

                actionContext.Response.ReasonPhrase = "Identity lacks required claims";
                return Task.FromResult(0);
            }

            return base.OnAuthorizationAsync(actionContext, cancellationToken);
        }
    }
}