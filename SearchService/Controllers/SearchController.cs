using InventorySearchApi.Models;
using InventorySearchApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace InventorySearchApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SearchController : ControllerBase
{
    private readonly SearchService _searchService;

    public SearchController(SearchService searchService)
    {
        _searchService = searchService;
    }

    [HttpGet("search")]
    public async Task<IActionResult> Search([FromQuery] string q)
    {
        var results = await _searchService.SearchAsync(q);
        return Ok(results);
    }

    [HttpGet("suggest")] 
    public async Task<IActionResult> Suggest([FromQuery] string term)
    {
        var suggestions = await _searchService.SuggestAsync(term);
        return Ok(suggestions);
    }

    [HttpPost("seed")]
    public async Task<IActionResult> Seed()
    {
        await _searchService.CreateIndexIfNotExistsAsync();

        // Description, Category
        var products = new List<Product>
        {
            new() { Name = "Apple MacBook Pro 14-inch" },
            new() { Name = "Dell XPS 15" },
            new() { Name = "Lenovo ThinkPad X1 Carbon" },
            new() { Name = "Logitech MX Master 3 Mouse" },
            new() { Name = "Apple iPhone 15 Pro Max" },
            new() { Name = "Samsung Galaxy S24 Ultra" },
            new() { Name = "Sony WH-1000XM5 Headphones" },
            new() { Name = "HP Spectre x360" },
            new() { Name = "Razer Blade 16 Gaming Laptop" },
            new() { Name = "Google Pixel 8 Pro" },
            new() { Name = "Microsoft Surface Laptop 5" },
            new() { Name = "Bose QuietComfort Ultra Earbuds" },
            new() { Name = "Anker PowerCore 20000mAh Power Bank" },
            new() { Name = "ASUS ROG Strix Scar 17" },
            new() { Name = "Canon EOS R7 Mirrorless Camera" }
        };

        await _searchService.UploadProductsAsync(products);
        return Ok("Seeded successfully");
    }
}