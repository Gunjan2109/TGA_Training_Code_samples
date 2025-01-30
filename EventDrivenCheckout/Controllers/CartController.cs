using EventDrivenCheckout.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace EventDrivenCheckout.Controllers
{
    [Route("api/cart")]
    [ApiController]
    public class CartController : ControllerBase
    {
        public static readonly List<CartItem> Cart = new();

        [HttpPost("add")]
        public ActionResult AddToCart(int productId, int quantity)
        {
            var product = ProductsController.Products.FirstOrDefault(p => p.Id == productId);
            if (product == null || quantity > product.QuantityAvailable)
                return BadRequest("Invalid product ID or insufficient stock.");

            var cartItem = Cart.FirstOrDefault(c => c.Products.Id == productId);
            if (cartItem == null)
            {
                Cart.Add(new CartItem { Products = product, Quantity = quantity });
            }
            else
            {
                if (cartItem.Quantity + quantity > product.QuantityAvailable)
                    return BadRequest("Insufficient stock to update cart.");
                cartItem.Quantity += quantity;
            }

            return Ok("Product added to cart.");
        }

        [HttpGet]
        public ActionResult<IEnumerable<CartItem>> ViewCart() => Ok(Cart);
    }

}
