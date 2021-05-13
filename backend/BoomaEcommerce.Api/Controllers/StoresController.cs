using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using BoomaEcommerce.Api.Requests;
using BoomaEcommerce.Api.Responses;
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
        private readonly IMapper _mapper;

        public StoresController(IStoresService storeService, IMapper mapper)
        {
            _storeService = storeService;
            _mapper = mapper;
        }

        [Authorize]
        [HttpPost(ApiRoutes.Stores.Products.Post)]
        public async Task<IActionResult> CreateProduct(Guid storeGuid, [FromBody] ProductDto product)
        {
            product.StoreGuid = storeGuid;
            var productResult = await _storeService.CreateStoreProductAsync(product);

            if (productResult == null)
            {
                return BadRequest();
            }
            var locationUrl = $"{this.GetBaseUrl()}/products/{productResult.Guid}";
            return Created(locationUrl, productResult);
        }

        [Authorize]
        [HttpDelete(ApiRoutes.Stores.Products.Delete)]
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
        [HttpPut(ApiRoutes.Stores.Products.Put)]
        public async Task<IActionResult> UpdateProduct(Guid storeGuid, Guid productGuid, ProductDto product)
        {
            product.Guid = productGuid;
            product.StoreGuid = storeGuid;
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

        [HttpGet(ApiRoutes.Stores.Products.Get)]
        public async Task<IActionResult> GetStoreProduct(Guid productGuid)
        {
            var product = await _storeService.GetStoreProductAsync(productGuid);
            if (product == null)
            {
                return NotFound();
            }

            return Ok(product);
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

        [Authorize]
        [HttpGet(ApiRoutes.Stores.Roles.Get)]
        public async Task<IActionResult> GetStoreRoles(Guid storeGuid)
        {
            var storeSellers = await _storeService.GetAllSellersInformationAsync(storeGuid);
            if (storeSellers == null)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }

            return Ok(_mapper.Map<StoreSellersResponse>(storeSellers));
        }

        [Authorize]
        [HttpPost(ApiRoutes.Stores.Roles.Ownerships.Post)]
        public async Task<IActionResult> PostOwnershipRole(Guid storeGuid, [FromBody] CreateOwnershipRequest request)
        {
            var nominatedOwnership = _mapper.Map<StoreOwnershipDto>(request,
                opt => opt.AfterMap((_, dest) => dest.Store.Guid = storeGuid));

            var result = await _storeService.NominateNewStoreOwnerAsync(request.NominatingOwnershipGuid, nominatedOwnership);

            if (result)
            {
                return Ok();
            }

            return BadRequest();
        }

        [Authorize]
        [HttpPost(ApiRoutes.Stores.Roles.Managements.Post)]
        public async Task<IActionResult> PostOwnershipRole(Guid storeGuid, [FromBody] CreateManagementRequest request)
        {
            var nominatedOwnership = _mapper.Map<StoreManagementDto>(request,
                opt => opt.AfterMap((_, dest) => dest.Store.Guid = storeGuid));

            var result = await _storeService.NominateNewStoreManagerAsync(request.NominatingOwnershipGuid, nominatedOwnership);

            if (result)
            {
                return Ok();
            }

            return BadRequest();
        }
    }
}
