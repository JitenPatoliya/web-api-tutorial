using System.Web.Http.Controllers;
using System.Web.Http.ModelBinding;

namespace HandlerTemplates.Models
{
    public class ModelBinderTemplate : IModelBinder
    {
        public bool BindModel(HttpActionContext actionContext, 
            ModelBindingContext bindingContext)
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
                        // STEP 1: do your processing on the attempted value, and save the final
                        //         object to the bindingContext.Model
                        // TODO: for this sample we just copy the string, your logic 
                        //         should parse/process it
                        var obj = s as string;
                        bindingContext.Model = obj;
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