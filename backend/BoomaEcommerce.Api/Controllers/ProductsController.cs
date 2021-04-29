using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BoomaEcommerce.Services.DTO;
using BoomaEcommerce.Services.Products;
using BoomaEcommerce.Services.Stores;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace BoomaEcommerce.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductsController : ControllerBase
    {
        private readonly IProductsService _productService;
        private readonly IStoresService _storesService;

        public ProductsController(IProductsService productService, IStoresService storesService)
        {
            _productService = productService;
            _storesService = storesService;
        }

        [HttpGet(ApiRoutes.Products.Get)]
        public async Task<IActionResult> GetProduct(Guid productGuid)
        {
            var product = await _productService.GetProductAsync(productGuid);

            if (product == null)
            {
                return NotFound();
            }

            return Ok(product);
        }

        [HttpGet]
        public async Task<IActionResult> GetProducts()
        {
            var products = await _productService.GetAllProductsAsync();
            if (products == null)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }

            return Ok(products);
        }
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> CreateProduct([FromBody] ProductDto product)
        {
            var productResult = await _storesService.CreateStoreProductAsync(product);

            if (productResult == null)
            {
                return BadRequest();
            }

            return Ok(productResult);
        }

        [Authorize]
        [HttpDelete(ApiRoutes.Products.Delete)]
        public async Task<IActionResult> DeleteProduct(Guid productGuid)
        {
            var res = await _storesService.DeleteProductAsync(productGuid);
            if (res)
            {
                return NoContent();
            }

            return NotFound();
        }

        [Authorize]
        [HttpPut]
        public async Task<IActionResult> UpdateProduct(ProductDto product)
        {
            var res = await _storesService.UpdateProductAsync(product);
            if (res)
            {
                return NoContent();
            }
            return NotFound();
        }
    }
}
