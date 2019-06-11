using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;


namespace TestProject5.Filters
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false)]
    public class ValidateModelStateAttribute : ActionFilterAttribute
    {
        // True if the bound FromBody parameter is required (disallow nulls)
        public bool BodyRequired { get; set; }

        /// <summary>
        /// Executed BEFORE the controller action method is called
        /// </summary>
        public override async Task OnActionExecutingAsync(HttpActionContext actionContext, CancellationToken cancellationToken)
        {
            // STEP 1:  Any logic you want to do BEFORE the rest of the action filter chain is 
            //          called, and BEFORE the action method itself.
            if (!actionContext.ModelState.IsValid)
            {
                actionContext.Response = actionContext.Request.CreateErrorResponse(
                    HttpStatusCode.BadRequest, actionContext.ModelState);
            }

            // if the FromBody parameter is required, find it in the action arguments and check for null
            else if (BodyRequired)
            {
                foreach (var b in actionContext.ActionDescriptor.ActionBinding.ParameterBindings)
                {
                    if (b.WillReadBody)
                    {
                        if (!actionContext.ActionArguments.ContainsKey(b.Descriptor.ParameterName)
                            || actionContext.ActionArguments[b.Descriptor.ParameterName] == null)
                        {
                            actionContext.Response = actionContext.Request.CreateErrorResponse(
                                                HttpStatusCode.BadRequest, b.Descriptor.ParameterName + " is required.");
                        }
                        // since only one FromBody can exist, we can abort the loop after a body param is found
                        break;
                    }
                }
            }

            // STEP 2: Call the rest of the action filter chain
            await base.OnActionExecutingAsync(actionContext, cancellationToken);

            // STEP 3: Any logic you want to do AFTER the other action filters, but BEFORE
            //         the action method itself is called.
        }
    }
}