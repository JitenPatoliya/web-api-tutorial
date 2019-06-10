using HandlerTemplates.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace HandlerTemplates.Controllers
{
    [ActionFilterTemplate]
    [RoutePrefix("values")]
    public class ValuesController : ApiController
    {
        // GET <controller>
        [ActionFilterTemplate]
        [HttpGet, Route("")]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET <controller>/5
        [OverrideActionFilters]
        [ActionFilterTemplate]
        [HttpGet, Route("{id:int}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST <controller>
        [HttpPost, Route("")]
        public void Post([FromBody]string value)
        {
        }

        // PUT <controller>/5
        [HttpPut, Route("{id:int}")]
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE <controller>/5
        [HttpDelete, Route("{id:int}")]
        public void Delete(int id)
        {
        }
    }
}