using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BoomaEcommerce.Services.Products;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace BoomaEcommerce.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductsController : ControllerBase
    {
        private readonly ILogger<ProductsController> _logger;
        private readonly IProductsService _productService;

        public ProductsController(ILogger<ProductsController> logger, IProductsService productService)
        {
            _logger = logger;
            _productService = productService;
        }

        [HttpGet("{productGuid}")]
        public async Task<IActionResult> GetProduct(Guid productGuid)
        {
            var product = await _productService.GetProductAsync(productGuid);

            if (product == null)
            {
                return NotFound();
            }

            return Ok(product);
        }
    }
}
