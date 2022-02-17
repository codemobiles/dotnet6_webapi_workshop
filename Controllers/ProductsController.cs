using System.Net;
using System;
using dotnet_hero.Data;
using dotnet_hero.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using dotnet_hero.DTOs.Product;
using Mapster;
using dotnet_hero.Interfaces;
using Microsoft.AspNetCore.Authorization;

namespace dotnet_hero.Controllers;

[ApiController]
[Route("[controller]")]
[Authorize(Roles = "Admin, Cashier")]
public class ProductsController : ControllerBase
{
    private static readonly string[] Summaries = new[]
    {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    };

    private readonly ILogger<ProductsController> _logger;
    private readonly IProductService productService;

    public ProductsController(ILogger<ProductsController> logger, IProductService productService)
    {
        _logger = logger;
        this.productService = productService;
    }


    [HttpGet]
    public async Task<ActionResult<IEnumerable<ProductResponse>>> GetProductsAsync()
    {
        return (await productService.FindAll()).Select(ProductResponse.FromProduct).ToList();
    }


    [HttpGet("{id}")]
    public async Task<ActionResult<ProductResponse>> GetProductByIdAsync(int id)
    {
        var product = await productService.FindById(id);
        if (product == null)
        {
            return NotFound();
        }
        return ProductResponse.FromProduct(product);
    }


    [HttpGet("search")]
    public async Task<ActionResult<IEnumerable<ProductResponse>>> SearchProductAsync([FromQuery] string name)
    {
        var result = (await productService.Search(name))
        .Select(ProductResponse.FromProduct)
        .ToList();

        return result;
    }

    [HttpPost]
    public async Task<ActionResult<Product>> AddProductAsync([FromForm] ProductRequest productRequest)
    {

        (string errorMessage, string imageName) = await productService.UploadImage(productRequest.FormFiles);
        if (!String.IsNullOrEmpty(errorMessage)){
            return BadRequest();
        }

        var product = productRequest.Adapt<Product>();
        product.Image = imageName;
        await productService.Create(product);        
        return StatusCode((int)HttpStatusCode.Created, product);
    }


    [HttpPut("{id}")]
    public async Task<ActionResult> UpdateProductAsync(int id, [FromForm] ProductRequest productRequest)
    {
        if (id != productRequest.ProductId)
        {
            return BadRequest();
        }

        var product = await productService.FindById(id);
        if (product == null)
        {
            return NotFound();
        }

        (string errorMessage, string imageName) = await productService.UploadImage(productRequest.FormFiles);
        if (!String.IsNullOrEmpty(errorMessage)){
            return BadRequest();
        }
        if (!String.IsNullOrEmpty(imageName)){
            product.Image = imageName;
        }
        
        productRequest.Adapt(product);
        var result = productService.Update(product);        
        return Ok(ProductResponse.FromProduct(product));

    }


    [HttpDelete("{id}")]
    public async Task<ActionResult> DeletProductAsync(int id)
    {
        var product = await productService.FindById(id);
        if (product == null)
        {
            return NotFound();
        }

        await productService.Delete(product);
        return NoContent();
    }
}
