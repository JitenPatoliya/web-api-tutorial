using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using TestProject8.Filters;

namespace TestProject8.Controllers
{
    [RoutePrefix("values")]
    public class ValuesController : ApiController
    {
        // GET api/<controller>
        [HttpGet, Route("")]
        public IEnumerable<string> Get()
        {
            throw new InvalidOperationException();
            return new string[] { "value1", "value2" };
        }

        // GET api/<controller>/5
        [HttpGet, Route("{id:int}")]
        [InvalidAccountIdExceptionFilter]
        public string Get(int id)
        {
            throw new ArgumentOutOfRangeException("id", "IDs must be in the range 1 to 50");
            return "value";
        }

        // POST api/<controller>
        [HttpPost, Route("")]
        public void Post([FromBody]string value)
        {
            throw new NotImplementedException();
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