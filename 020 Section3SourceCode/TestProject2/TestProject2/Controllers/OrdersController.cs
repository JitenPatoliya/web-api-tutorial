using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace TestProject2.Controllers
{
    public class OrdersController : ApiController
    {
        // GET orders/<order Id>
        [HttpGet, Route("orders/{id:length(8)}", Order = 2)]
        public string GetOrderById(string id)
        {
            return "order-" + id;
        }

        // GET orders/pending,  orders/complete, etc.
        [HttpGet, Route("orders/{status:regex(^(?i)(new|complete|pending)$)}", Order = 1)]
        public IEnumerable<string> GetOrdersWithStatus()
        {
            return new string[] { "status1", "status2" };
        }
    }
}