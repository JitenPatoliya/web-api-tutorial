using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.Http.Routing;

namespace HandlerTemplates.Constraints
{
    /// <summary>
    /// Custom Web API route constraint to validate a parameter.
    /// </summary>
    /// <remarks>
    /// You will need to register the constraint in your WebApiConfig.cs file:
    /// <code>
    ///   var constraintResolver = new DefaultInlineConstraintResolver();
    ///   constraintResolver.ConstraintMap.Add("myConstraint", typeof(ParameterConstraintTemplate));
    ///   config.MapHttpAttributeRoutes(constraintResolver);
    /// </code>
    /// </remarks>
    public class ParameterConstraintTemplate : IHttpRouteConstraint

    {
        public const string DefaultConstraintName = "myConstraint";

        // TODO: If you need constructor arguments, create properties to hold them
        //       and public constructors that accept them.
        public ParameterConstraintTemplate()
        { }

        /// <summary>
        /// IHttpRouteConstraint.Match implementation to validate a parameter
        /// </summary>
        public bool Match(HttpRequestMessage request, IHttpRoute route, string parameterName,
            IDictionary<string, object> values, HttpRouteDirection routeDirection)
        {
            object value;

            if (values.TryGetValue(parameterName, out value) && value != null)
            {
                var stringVal = value as string;
                if (!String.IsNullOrEmpty(stringVal))
                {
                    // STEP 1: validate the parameter using some custom logic, return true if valid
                    // TODO: replace the if (true) with your custom validation logic.
                    if (true) 
                    {
                        return true;
                    }
                }
            }

            return false;
        }
    }
}