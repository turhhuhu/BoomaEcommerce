using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BoomaEcommerce.Services.DTO;
using BoomaEcommerce.Services.Stores;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
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
            var locationUrl = $"{this.GetBaseUrl()}/products/{productResult.Guid}";
            return Created(locationUrl, productResult);
        }

        [Authorize]
        [HttpDelete(ApiRoutes.Products.Delete)]
        public async Task<IActionResult> DeleteProduct(Guid productGuid)
        {
            var res = await _storeService.DeleteProductAsync(productGuid);
            if (res)
            {
                return NoContent();
            }

            return NotFound();
        }

        [Authorize]
        [HttpPut(ApiRoutes.Products.Put)]
        public async Task<IActionResult> UpdateProduct(ProductDto product)
        {
            var res = await _storeService.UpdateProductAsync(product);
            if (res)
            {
                return NoContent();
            }
            return NotFound();
        }

        [HttpGet(ApiRoutes.Stores.GetAllProducts)]
        public async Task<IActionResult> GetStoreProducts(Guid storeGuid)
        {
            var storesRes = await _storeService.GetProductsFromStoreAsync(storeGuid);
            if (storesRes == null)
            {
                return NotFound();
            }

            return Ok(storesRes);
        }

        [HttpGet(ApiRoutes.Stores.Get)]
        public async Task<IActionResult> GetStore(Guid storeGuid)
        {
            var store = await _storeService.GetStoreAsync(storeGuid);
            if (store == null)
            {
                return NotFound();
            }

            return Ok(store);
        }

        [HttpGet]
        public async Task<IActionResult> GetStores()
        {
            var stores = await _storeService.GetStoresAsync();
            if (stores == null)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }

            return Ok(stores);
        }
    }
}
