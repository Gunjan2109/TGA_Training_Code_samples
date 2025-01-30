using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ECommerceMockUp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentsController : ControllerBase
    {
        [HttpPost("checkout")] // api/payments/checkout
        public ActionResult Checkout()
        {
            if (!CartController.Cart.Any())
                return BadRequest("Cart is empty.");

            decimal totalAmount = 0;
            foreach (var item in CartController.Cart)
            {
                totalAmount += item.Products.Price * item.Quantity;
                item.Products.QuantityAvailable -= item.Quantity;
            }

            CartController.Cart.Clear();

            return Ok(new { Message = "Payment successful.", AmountCharged = totalAmount });
        }
    }

}
