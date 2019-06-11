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
    /// Custom model binder for a string array
    /// </summary>
    public class StringArrayWildcardBinder : IModelBinder
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
                        // parse the elements on the forward slash
                        var array = s.Split('/');
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