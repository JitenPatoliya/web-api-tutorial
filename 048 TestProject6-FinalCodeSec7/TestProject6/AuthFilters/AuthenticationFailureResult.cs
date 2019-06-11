using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;

namespace TestProject6.AuthFilters
{
    /// <summary>
    /// Generic authentication failure action result, for use in authentication filters
    /// </summary>
    public class AuthenticationFailureResult : IHttpActionResult
    {
        /// <summary>
        /// Optional override value for the ReasonPhrase on the final
        /// 401 Unauthorized response, i.e. replaces "Unauthorized".
        /// </summary>
        public string ReasonPhrase { get; private set; }

        /// <summary>
        /// HttpRequestMessage for this response
        /// </summary>
        public HttpRequestMessage Request { get; private set; }

        /// <summary>
        /// Constructor taking only the original HttpRequestMessage
        /// </summary>
        public AuthenticationFailureResult(HttpRequestMessage request)
        {
            ReasonPhrase = null;
            Request = request;
        }

        /// <summary>
        /// Constructor taking an override value for ReasonPhrase to use instead of "Unauthorized",
        /// plus the original HttpRequestMessage
        /// </summary>
        public AuthenticationFailureResult(string reasonPhrase, HttpRequestMessage request)
        {
            ReasonPhrase = reasonPhrase;
            Request = request;
        }

        /// <summary>
        /// IHttpActionResult implementation to retrieve the HttpResponseMessage result
        /// </summary>
        public Task<HttpResponseMessage> ExecuteAsync(CancellationToken cancellationToken)
        {
            return Task.FromResult(Execute());
        }

        /// <summary>
        /// Internal method to manually create a new HttpResponseMessage containing
        /// the 401 status code and the reason phrase.
        /// </summary>
        private HttpResponseMessage Execute()
        {
            HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.Unauthorized);
            response.RequestMessage = Request;

            if (!String.IsNullOrEmpty(ReasonPhrase))
                response.ReasonPhrase = ReasonPhrase;
            else
                response.ReasonPhrase = Enum.GetName(typeof(HttpStatusCode), HttpStatusCode.Unauthorized);

            return response;
        }
    }
}