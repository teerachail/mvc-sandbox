using System;
using Microsoft.AspNet.Mvc;

namespace MediumApi.Controllers
{
    [Route("/store")]
    public class StoreController : BaseController
    {
        [HttpGet("inventory")]
        public IActionResult GetInventory()
        {
            throw new NotImplementedException();
        }

        [HttpGet("order/{orderId}")]
        public IActionResult FindOrder(int orderId)
        {
            throw new NotImplementedException();
        }

        [HttpPost("order")]
        public IActionResult PlaceOrder()
        {
            throw new NotImplementedException();
        }

        [HttpDelete("order/{orderId}")]
        public IActionResult CancelOrder(int orderId)
        {
            throw new NotImplementedException();
        }
    }
}
