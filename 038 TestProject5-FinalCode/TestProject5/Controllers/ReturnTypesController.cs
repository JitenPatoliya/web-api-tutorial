using Newtonsoft.Json;
using Swashbuckle.Swagger.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web.Http;
using System.Web.Http.Description;
using TestProject5.Filters;
using TestProject5.Models;
using TestProject5.ActionResults;

namespace TestProject5.Controllers
{
    [RoutePrefix("returntypes")]
    [ClientCacheControlFilter(ClientCacheControl.NoCache)]
    public class ReturnTypesController : ApiController
    {
        #region Traditional return types
        /// <summary>
        /// void returns get converted to a 204 No Content response message
        /// </summary>
        [HttpGet, Route("void")]
        public void ReturnVoid()
        {
        }

        /// <summary>
        /// the simple object instance return type, here a list of strings
        /// </summary>
        [HttpGet, Route("object")]
        public ComplexTypeDto GetObject()
        {
            var dto = new ComplexTypeDto()
            {
                String1 = "This is string 1",
                String2 = "This is string 2",
                Int1 = 55,
                Date1 = DateTime.Now
            };

            // error response:
            throw new InvalidOperationException("I'm sorry, Dave, I'm afraid I can't do that.");

            return dto;
        }

        /// <summary>
        /// Returns an HttpResponseMessage
        /// </summary>
        [HttpGet, Route("httpresponse")]
        [ResponseType(typeof(ComplexTypeDto))]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(ComplexTypeDto))]
        [SwaggerResponse(HttpStatusCode.BadRequest, Type = typeof(HttpError))]
        public HttpResponseMessage GetAnHttpResponse()
        {
            var dto = new ComplexTypeDto()
            {
                String1 = "This is string 1",
                String2 = "This is string 2",
                Int1 = 55,
                Date1 = DateTime.Now
            };

            //var response = new HttpResponseMessage(HttpStatusCode.OK)
            //{
            //    // note I am responsible for my own content negotiation!
            //    // this content will confuse a caller wanting XML or other media type
            //    Content = new StringContent(JsonConvert.SerializeObject(dto), Encoding.UTF8, "application/json")
            //};

            // or alternatively
            var response = Request.CreateResponse(dto);

            response.Headers.Add("X-MyCustomHeader", "MyHeaderValue");
            response.ReasonPhrase = "Most Excellent!";

            // error response
            response = Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Invalid something or other");

            return response;
        }
        #endregion

        /// <summary>
        /// Returns an IHttpActionResult
        /// </summary>
        [HttpGet, Route("actionresult")]
        [ResponseType(typeof(ComplexTypeDto))]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(ComplexTypeDto))]
        public IHttpActionResult GetAnActionResult()
        {
            var dto = new ComplexTypeDto()
            {
                String1 = "This is string 1",
                String2 = "This is string 2",
                Int1 = 55,
                Date1 = DateTime.Now
            };

            //var response = Json(dto);
            var response = Ok(dto).AddHeader("X-MyCustomHeader", "test value");
            
            //var response = BadRequest("test test test").AddHeader("X-MyCustomHeader", "test value"); 

            return response;
        }



    }
}
