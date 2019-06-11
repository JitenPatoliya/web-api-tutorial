using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;

namespace TestProject2.Controllers
{
    [RoutePrefix("products")]
    public class ProductsController : ApiController
    {
        [JsonConverter(typeof(StringEnumConverter))]
        public enum Widgets
        {
            Bolt,
            Screw,
            Nut,
            Motor
        };

        // GET: Products/widget/xxx
        [HttpGet, Route("widget/{widget:enum(TestProject2.Controllers.ProductsController+Widgets)}")]
        public string GetProductsWithWidget(Widgets widget)
        {
            return "widget-" + widget.ToString();
        }

        [HttpGet, Route("status/{status:alpha=}")]
        public string GetProductsWithStatus(string status)
        {
            return String.IsNullOrEmpty(status) ? "NULL" : status;
        }

        // GET: api/Products
        [HttpGet, Route("")]
        public IEnumerable<string> GetAllProducts()
        {
            return new string[] { "product1", "product2" };
        }
        
        // GET: api/Products/5
        [HttpGet, Route("{id:int:range(1000,3000)}", Name = "GetById")]
        public string GetProduct(int id)
        {
            return "product-" + id;
        }


        // GET: api/Products/5/orders/custid
        [HttpGet, Route("{id:int:range(1000,3000)}/orders/{custid}")]
        public string GetProductForCustomer(int id, string custid)
        {
            return "product-orders-" + custid;
        }
        
        // POST: api/Products
        [HttpPost, Route("")]
        public void CreateProduct([FromBody]string value)
        {
        }

        [HttpPost, Route("{prodId:int:range(1000,3000)}")]
        public HttpResponseMessage CreateProduct([FromUri]int prodId)
        {
            // do some create logic, then...

            var response = Request.CreateResponse(HttpStatusCode.Created);

            // Generate a link to the new book and set the Location header in the response.
            string uri = Url.Link("GetById", new { id = prodId });
            response.Headers.Location = new Uri(uri);
            return response;
        }


        // PUT: api/Products/5
        [HttpPut, Route("{id:int:range(1000,3000)}")]
        public void UpdateProduct([FromUri]int id, [FromBody]string value)
        {
        }

        // DELETE: api/Products/5
        [HttpDelete, Route("{id:int:range(1000,3000)}")]
        public void DeleteProduct(int id)
        {
        }
    }
}
