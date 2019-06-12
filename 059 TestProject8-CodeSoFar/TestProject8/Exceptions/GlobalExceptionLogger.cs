using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http.ExceptionHandling;


namespace TestProject8.Exceptions
{
    /// <summary>
    /// Global unhandled exception logging/analytics template
    /// </summary>
    /// <remarks>
    /// To register one or more loggers:
    /// <code>
    /// config.Services.Add(typeof(IExceptionLogger), new GlobalExceptionLoggerTemplate());
    /// </code>
    /// </remarks>
    public class GlobalExceptionLogger : ExceptionLogger
    {
        /// <summary>
        /// Required ExceptionLogger method to process an exception
        /// </summary>
        /// <remarks>
        /// Important! Not every ExceptionLoggerContext field will be set depending on where
        /// the exception occurs, but you can minimally count on the Exception and Request properties.
        /// </remarks>
        public override Task LogAsync(ExceptionLoggerContext context, CancellationToken cancellationToken)
        {
            //throw new ArgumentNullException();

            // STEP 1: do whatever analytics you like on the exception
            var ex = context.Exception;

            // example - simple trace logging
            Trace.WriteLine("*** Exception: " + ex.ToString());

            return Task.FromResult(0);
        }
    }
}