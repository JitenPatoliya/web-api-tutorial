using Swashbuckle.Swagger.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using TestProject5.Filters;
using TestProject5.Models;

namespace TestProject5.Controllers
{
    [RoutePrefix("models")]
    public class ModelsController : ApiController
    {
        [HttpPost, Route("object")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(string))]
        [ValidateModelState(BodyRequired =true)]    // METHOD 2: Use an Action Filter
        public IHttpActionResult Post([FromBody]ComplexTypeDto dto)
        {
            // METHOD 1: Inline model validation
            //if (!ModelState.IsValid)
            //{
            //    return BadRequest(ModelState);
            //}

            return Ok("Posted data valid");
        }

    }
}