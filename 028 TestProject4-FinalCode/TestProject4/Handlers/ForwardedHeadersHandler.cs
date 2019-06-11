using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace TestProject4.Handlers
{
    public class ForwardedHeadersHandler : DelegatingHandler
    {
        // new style header:   "Forwarded: by=<identifier>; for=<identifier>; host=<host>; proto=<http|https>"  
        //https://developer.mozilla.org/en-US/docs/Web/HTTP/Headers/Forwarded
        const string _fwdHeader = "Forwarded";

        // old style, separate headers
        //https://developer.mozilla.org/en-US/docs/Web/HTTP/Headers/X-Forwarded-Proto
        const string _fwdProtoHeader = "X-Forwarded-Proto";
        //https://developer.mozilla.org/en-US/docs/Web/HTTP/Headers/X-Forwarded-Host
        const string _fwdHostHeader = "X-Forwarded-Host";

        /// <summary>
        /// Property name for storing the self-referencing base URL  
        /// of this service from the client's perspective
        /// </summary>
        public const string MyClientBaseUrlProperty = "MyClientBaseUrl";

        /// <summary>
        /// This handler processes both old and new-style Forwarded headers
        /// to calculate the "self-referencing" base URL from 
        /// the client's perspective, i.e. the url the client used to call
        /// the service. This base can be used to create new URLs sent
        /// back to the client for it to call other methods of your service.
        /// </summary>
        /// <remarks>
        /// If a load balancer sits in front of your service, then your service is
        /// called by the load balancer not the actual client. This means you
        /// need to examine the Forwarded headers to know what the client 
        /// actually called, not the local request values.
        ///
        /// To simplify retrieving the client base url in your code I include extensions
        /// on HttpRequestMessage below.
        /// </remarks>
        protected override Task<HttpResponseMessage> SendAsync(
              HttpRequestMessage request, CancellationToken cancellationToken)
        {
            // first, let's start with a basic URI based on the server's view of the request
            UriBuilder builder = new UriBuilder(request.RequestUri.Scheme,
                                                request.RequestUri.Host,
                                                request.RequestUri.Port);

            // override host and protocol if found in headers
            // first check the legacy x-forwarded headers
            if (request.Headers.Contains(_fwdProtoHeader))
            {
                // the protocol - http or https
                var proto = request.Headers.GetValues(_fwdProtoHeader)
                        .FirstOrDefault(s => !String.IsNullOrEmpty(s));
                if (proto != null)
                    builder.Scheme = proto;
            }
            if (request.Headers.Contains(_fwdHostHeader))
            {
                // the forwarded host string 
                var host = request.Headers.GetValues(_fwdHostHeader)
                    .FirstOrDefault(s => !String.IsNullOrEmpty(s));
                if (host != null)
                    SetHostAndPort(builder, host);
            }
            // next try the newer Forwarded header
            if (request.Headers.Contains(_fwdHeader))
            {
                // grab the forward host string 
                var fwd = request.Headers.GetValues(_fwdHeader)
                    .FirstOrDefault(s => !String.IsNullOrEmpty(s))
                    .Split(';')
                    .Select(s => s.Trim());

                // syntax for the Forwarded header: Forwarded: by=<identifier>; for=<identifier>; host=<host>; proto=<http|https>
                var proto = fwd.FirstOrDefault(s => s.ToLowerInvariant().StartsWith("proto="));
                if (!String.IsNullOrEmpty(proto))
                {
                    proto = proto.Substring(6).Trim();
                    if (!String.IsNullOrEmpty(proto))
                        builder.Scheme = proto;
                }

                var host = fwd.FirstOrDefault(s => s.ToLowerInvariant().StartsWith("host="));
                if (!String.IsNullOrEmpty(host))
                {
                    host = host.Substring(5).Trim();
                    if (!String.IsNullOrEmpty(host))
                        SetHostAndPort(builder, host);
                }
            }

            builder.Path = "/";

            // add the final calculated URL to the Properties collection
            request.Properties.Add(MyClientBaseUrlProperty, builder.Uri);

            // continue the handler chain
            return base.SendAsync(request, cancellationToken);
        }

        private static void SetHostAndPort(UriBuilder builder, string host)
        {
            var hostAndPort = host.Split(':');
            builder.Host = hostAndPort[0];
            if (hostAndPort.Length > 1)
                builder.Port = int.Parse(hostAndPort[1]);
            else
                builder.Port = -1;
        }
    }

    /// <summary>
    /// HttpRequestMessage extension to get the base URL of the service from the client's perspective
    /// </summary>
    public static class HttpRequestMessageBaseUrlExtension
    {
        /// <summary>
        /// Retrieves the base URL to use in order to create a "self-referencing URL", a URL
        /// that points at this web service but from the client's perspective, taking into
        /// account any load balancers in use in front of the service.
        /// </summary>
        /// <remarks>
        /// Assuming the original URL the client browser called was:  https://www.mycompany.com/api/products
        /// but your web service sits behind a load balancer at:  http://myserver/api/products
        /// the self-reference base URL returned by this method would be: https://www.mycompany.com/
        /// 
        /// If no load balancer was in use then the self-reference base URL 
        /// returned by this method would be: http://myserver/ 
        /// </remarks>
        /// <param name="request">HttpRequestMessage object.</param>
        /// <returns>Self-referencing base URL for creating another URL that references the same service,
        /// from the original client caller's perspective.</returns>
        public static Uri GetSelfReferenceBaseUrl(this HttpRequestMessage request)
        {
            if (request == null)
                return null;

            if (request.Properties.TryGetValue(ForwardedHeadersHandler.MyClientBaseUrlProperty,
                out object baseUrl))
            {
                return (Uri)baseUrl;
            }

            return null;
        }

        /// <summary>
        /// Retrieves a URL re-based from the client's perspective, taking into
        /// account any load balancers in use in front of the service, given a server-base URL.
        /// </summary>
        /// <remarks>
        /// See remarks on <see cref="GetSelfReferenceBaseUrl"/>.
        /// </remarks>
        /// <param name="request">HttpRequestMessage object.</param>
        /// <param name="serverUrl">Uri instance of the server-based URL</param>
        /// <returns>Re-based URL from the original client caller's perspective.</returns>
        public static Uri RebaseUrlForClient(this HttpRequestMessage request, Uri serverUrl)
        {
            Uri clientBase = GetSelfReferenceBaseUrl(request);
            if (clientBase == null)
                return null;
            if (serverUrl == null)
                return clientBase;

            // rest the base scheme/host/port to the client version
            var builder = new UriBuilder(serverUrl);
            builder.Scheme = clientBase.Scheme;
            builder.Host = clientBase.Host;
            builder.Port = clientBase.Port;

            return builder.Uri;
        }
    }
}