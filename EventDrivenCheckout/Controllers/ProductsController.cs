using EventDrivenCheckout.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace EventDrivenCheckout.Controllers
{
    [Route("api/products")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        public static readonly List<Product> Products = new()
        {
            new Product { Id = 1, Name = "Laptop", Price = 1200.00m, QuantityAvailable = 10 },
            new Product { Id = 2, Name = "Headphones", Price = 200.00m, QuantityAvailable = 15 },
            new Product { Id = 3, Name = "Smartphone", Price = 900.00m, QuantityAvailable = 8 }
        };

        [HttpGet]
        public ActionResult<IEnumerable<Product>> GetProducts() => Ok(Products);
    }
}
