using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BoomaEcommerce.Services.DTO;
using BoomaEcommerce.Services.Stores;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;

namespace BoomaEcommerce.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class StoresController : ControllerBase
    {
        private readonly IStoresService _storeService;

        public StoresController(IStoresService storeService)
        {
            _storeService = storeService;
        }

        [Authorize]
        [HttpPost(ApiRoutes.Products.Post)]
        public async Task<IActionResult> CreateProduct([FromBody] ProductDto product)
        {
            var productResult = await _storeService.CreateStoreProductAsync(product);

            if (productResult == null)
            {
                return BadRequest();
            }

            return Ok(productResult);
        }
    }
}
