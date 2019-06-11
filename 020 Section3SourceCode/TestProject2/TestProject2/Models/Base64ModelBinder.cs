using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http.Controllers;
using System.Web.Http.ModelBinding;
using System.Web.Http.Routing.Constraints;

namespace TestProject2.Models
{
    /// <summary>
    /// Simple regex constraint to verify the characters are from
    /// the modified base64 set
    /// </summary>
    public class Base64Constraint : RegexRouteConstraint
    {
        public Base64Constraint() : base("^([A-Za-z0-9+/\\-_])*={0,3}$")
        {
        }
    }


    /// <summary>
    /// Custom model binder for a base64 string, binds to a byte[] array
    /// </summary>
    public class Base64ParameterModelBinder : IModelBinder
    {
        public bool BindModel(HttpActionContext actionContext, ModelBindingContext bindingContext)
        {
            var key = bindingContext.ModelName;
            var val = bindingContext.ValueProvider.GetValue(key);
            if (val != null)
            {
                var s = val.AttemptedValue;
                if (s != null)
                {
                    try
                    {
                        // support "modified base64" where + was replaced with -, and / replaced with _
                        // since this is a URL parameter.  Caller can URL-encoded as an alternative also.
                        s = s.Replace('-', '+').Replace('_', '/');
                        var array = Convert.FromBase64String(s);
                        bindingContext.Model = array;
                    }
                    catch
                    {
                        return false;
                    }
                }
                return true;
            }
            return false;
        }
    }
}