using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http.Filters;
using System.Net.Http.Headers;

namespace TestProject5.Filters
{
    /// <summary>
    /// Enum for type of client side caching
    /// </summary>
    /// <remarks> See this article for details of each:
    /// https://developers.google.com/web/fundamentals/performance/optimizing-content-efficiency/http-caching
    /// </remarks>
    public enum ClientCacheControl
    {
        Public,     // can be cached by intermediate devices even if authentication was used;
        Private,    // browser-only, no intermediate caching, typically for per-user data
        NoCache     // no caching by browser or intermediate devices
    };

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false)]
    public class ClientCacheControlFilterAttribute : ActionFilterAttribute
    {
        // TODO: If you need constructor arguments, create properties to hold them
        //       and public constructors that accept them.
        public ClientCacheControl CacheType;
        public double CacheSeconds;


        public ClientCacheControlFilterAttribute(double seconds = 60.0)
        {
            CacheType = ClientCacheControl.Private;
            CacheSeconds = seconds;
        }

        public ClientCacheControlFilterAttribute(ClientCacheControl cacheType, double seconds = 60.0)
        {
            CacheType = cacheType;
            CacheSeconds = seconds;
            if (cacheType == ClientCacheControl.NoCache)
                CacheSeconds = -1;
        }


        public override async Task OnActionExecutedAsync(HttpActionExecutedContext actionExecutedContext, CancellationToken cancellationToken)
        {
            // STEP 2: Call the rest of the action filter chain
            await base.OnActionExecutedAsync(actionExecutedContext, cancellationToken);

            // STEP 3: Any logic you want to do AFTER the other action filters, and AFTER
            //         the action method itself is called.
            if (actionExecutedContext.Response == null)
                return;

            if (CacheType == ClientCacheControl.NoCache)
            {
                actionExecutedContext.Response.Headers.CacheControl = new CacheControlHeaderValue
                {
                    NoStore = true
                };

                // for older browsers
                actionExecutedContext.Response.Headers.Pragma.TryParseAdd("no-cache");

                // create a date if none present, so we can have Expires match it
                if (!actionExecutedContext.Response.Headers.Date.HasValue)
                    actionExecutedContext.Response.Headers.Date = DateTimeOffset.UtcNow;

                if (actionExecutedContext.Response.Content != null)
                    actionExecutedContext.Response.Content.Headers.Expires = 
                        actionExecutedContext.Response.Headers.Date;
            }
            else
            {
                actionExecutedContext.Response.Headers.CacheControl = new CacheControlHeaderValue
                {
                    Public = (CacheType == ClientCacheControl.Public),
                    Private = (CacheType == ClientCacheControl.Private),
                    NoCache = false,
                    MaxAge = TimeSpan.FromSeconds(CacheSeconds)
                };
                // create a date if none present, so we can have Expires match it
                if (!actionExecutedContext.Response.Headers.Date.HasValue)
                    actionExecutedContext.Response.Headers.Date = DateTimeOffset.UtcNow;

                if (actionExecutedContext.Response.Content != null)
                    actionExecutedContext.Response.Content.Headers.Expires =
                        actionExecutedContext.Response.Headers.Date;
            }
        }
    }
}