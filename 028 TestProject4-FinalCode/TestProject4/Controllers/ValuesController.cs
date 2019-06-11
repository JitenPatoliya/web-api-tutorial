using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using TestProject4.Handlers;

namespace TestProject4.Controllers
{
    [RoutePrefix("values")]
    public class ValuesController : ApiController
    {
        // GET api/<controller>
        [HttpGet, Route("")]
        public IEnumerable<string> Get()
        {
            var getByIdUrl = Url.Link("GetById", new { id = 123});

            //return new string[] { "value1", "value2" };
            return new string[] {
                getByIdUrl,
                Request.GetSelfReferenceBaseUrl().ToString(),
                Request.RebaseUrlForClient(new Uri(getByIdUrl)).ToString()
            };
        }

        // GET api/<controller>/5
        [HttpGet, Route("{id:int}", Name = "GetById")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/<controller>
        [HttpPost, Route("")]
        public void Post([FromBody]string value)
        {
        }

        // PUT api/<controller>/5
        [HttpPut, Route("{id:int}")]
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/<controller>/5
        [HttpDelete, Route("{id:int}")]
        public void Delete(int id)
        {
        }
    }
}