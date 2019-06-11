using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using TestProject6.AuthFilters;

namespace TestProject6.Controllers
{
    [RoutePrefix("values")]

    public class ValuesController : ApiController
    {
        // GET api/<controller>
        [Route("")]
        [RequireClaim("MyCustomClaim", IncludeMissingInResponse = true)]
        public IEnumerable<string> Get()
        {
            return new string[] { User.Identity.Name, User.Identity.AuthenticationType };
        }

        // GET api/<controller>/5
        public string Get(int id)
        {
            return "value";
        }

        // POST api/<controller>
        public void Post([FromBody]string value)
        {
        }

        // PUT api/<controller>/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/<controller>/5
        public void Delete(int id)
        {
        }
    }
}