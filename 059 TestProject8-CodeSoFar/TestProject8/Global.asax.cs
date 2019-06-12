using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Routing;

namespace TestProject8
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            GlobalConfiguration.Configuration.IncludeErrorDetailPolicy = 
                IncludeErrorDetailPolicy.Default;

            GlobalConfiguration.Configure(WebApiConfig.Register);
            //throw new ArgumentNullException();
        }

        /// <summary>
        /// For errors outside of the web api pipeline - start up and shut down errors,
        /// plus exceptions that bubble up from the global exception handler/loggers
        /// </summary>
        protected void Application_Error(object sender, EventArgs e)
        {
            Exception ex = Server.GetLastError();

            // since this is the handler of last resort - 
            //  you should probably log the error somewhere that will get noticed!
            Context.Trace.Write("ERROR-- WebApiApplication.Application_OnError reached: " 
                + ex.ToString());
        }
    }
}
