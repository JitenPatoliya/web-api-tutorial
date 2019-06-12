using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Net.Http;
using System.Web.Http.ExceptionHandling;
using System.Net;
using System.Web.Http.Results;

namespace TestProject8.Exceptions
{
    /// <summary>
    /// Global exception handler to remove the stack trace from all exceptions
    /// </summary>
    /// <remarks>
    /// To register, in WebApiConfig.cs:
    /// <code>
    /// config.Services.Replace(typeof(IExceptionHandler), new GlobalExceptionHandler());
    /// </code>
    /// Note you could daisy-chain several handlers together using the "inner handler" model
    /// if you need more than the one Web Api allows you to register. Just add a constructor
    /// taking the inner handler, and a property to hold it.
    /// </remarks>
    public class GlobalExceptionHandler : ExceptionHandler
    {
        /// <summary>
        /// Required ExceptionHandler method to process an exception
        /// </summary>
        /// <remarks>
        /// Important! Not every ExceptionHandlerContext field will be set depending on where
        /// the exception occurs, but you can minimally count on the Exception and Request properties.
        /// </remarks>
        public override Task HandleAsync(ExceptionHandlerContext context, CancellationToken cancellationToken)
        {
            // STEP 1: exit if we cannot handle the exception (boilerplate code)

            // nothing we can do if the context is not present
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }

            // verify this exception should be handled at all; exit if not
            if (!ShouldHandle(context))
            {
                return Task.FromResult(0);
            }

            // STEP 2: Create an IHttpActionResult from the exception as required
            var ex = context.Exception;

            // in this example, we simply strip off the stack trace and leave only the error message
            var responseMsg = context.Request.CreateErrorResponse(HttpStatusCode.InternalServerError,
                ex.Message);

            context.Result = new ResponseMessageResult(responseMsg);

            return Task.FromResult(0);
        }
    }
}