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
        private readonly ILogger<StoresController> _logger;
        private readonly IStoresService _storeService;

        public StoresController(ILogger<StoresController> logger, IStoresService storeService)
        {
            _logger = logger;
            _storeService = storeService;
        }

        [Authorize]
        [Route(ApiRoutes.Products.Post)]
        [HttpPost]
        public async Task<IActionResult> CreateProduct([FromBody] ProductDto product)
        {
            var productResult = await _storeService.CreateStoreProductAsync(product);

            if (productResult == null)
            {
                return BadRequest();
            }

            return Ok(productResult);
        }


        [HttpPost(ApiRoutes.Me)]
        public async Task<IActionResult> CreateStore([FromBody] StoreDto store)
        {
            var storeResult = await _storeService.CreateStoreAsync(store);

            if (storeResult == null)
            {
                return BadRequest();
            }

            return Ok(storeResult);
        }
    }
}
