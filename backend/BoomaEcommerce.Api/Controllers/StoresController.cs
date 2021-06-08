﻿using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using BoomaEcommerce.Api.Requests;
using BoomaEcommerce.Api.Responses;
using BoomaEcommerce.Core;
using BoomaEcommerce.Domain.Policies;
using BoomaEcommerce.Services.DTO;
using BoomaEcommerce.Services.DTO.Policies;
using BoomaEcommerce.Services.Stores;
using BoomaEcommerce.Services.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace BoomaEcommerce.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class StoresController : ControllerBase
    {
        private readonly IStoresService _storesService;
        private readonly IUsersService _userService;
        private readonly IMapper _mapper;

        public StoresController(IStoresService storesService, IMapper mapper, IUsersService userService)
        {
            _storesService = storesService;
            _mapper = mapper;
            _userService = userService;
        }

        [Authorize]
        [HttpPost(ApiRoutes.Stores.Products.Post)]
        public async Task<IActionResult> CreateProduct(Guid storeGuid, [FromBody] ProductDto product)
        {
            product.StoreGuid = storeGuid;
            var productResult = await _storesService.CreateStoreProductAsync(product);

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
            var res = await _storesService.DeleteProductAsync(productGuid);
            if (res)
            {
                return NoContent();
            }

            return NotFound();
        }

        [Authorize]
        [HttpPut(ApiRoutes.Stores.Products.Put)]
        public async Task<IActionResult> UpdateProduct(Guid storeGuid, Guid productGuid, [FromBody] ProductDto product)
        {
            product.Guid = productGuid;
            product.StoreGuid = storeGuid;
            var res = await _storesService.UpdateProductAsync(product);
            if (res)
            {
                return NoContent();
            }
            return NotFound();
        }

        [HttpGet(ApiRoutes.Stores.GetAllProducts)]
        public async Task<IActionResult> GetStoreProducts(Guid storeGuid)
        {
            var storesRes = await _storesService.GetProductsFromStoreAsync(storeGuid);
            if (storesRes == null)
            {
                return NotFound();
            }

            return Ok(storesRes);
        }

        [HttpGet(ApiRoutes.Stores.Products.Get)]
        public async Task<IActionResult> GetStoreProduct(Guid productGuid)
        {
            var product = await _storesService.GetStoreProductAsync(productGuid);
            if (product == null)
            {
                return NotFound();
            }

            return Ok(product);
        }

        [HttpGet(ApiRoutes.Stores.Get)]
        public async Task<IActionResult> GetStore(Guid storeGuid)
        {
            var store = await _storesService.GetStoreAsync(storeGuid);
            if (store == null)
            {
                return NotFound();
            }

            return Ok(store);
        }

        [HttpGet]
        public async Task<IActionResult> GetStores()
        {
            var stores = await _storesService.GetStoresAsync();
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
            var storeSellers = await _storesService.GetAllSellersInformationAsync(storeGuid);
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

            var result = await _storesService.NominateNewStoreOwnerAsync(request.NominatingOwnershipGuid, nominatedOwnership);

            if (result)
            {
                return Ok();
            }

            return BadRequest();
        }

        [Authorize]
        [HttpPost(ApiRoutes.Stores.Roles.Managements.Post)] 
        public async Task<IActionResult> PostManagementRole(Guid storeGuid, [FromBody] CreateManagementRequest request)
        {
            var nominatedOwnership = _mapper.Map<StoreManagementDto>(request,
                opt => opt.AfterMap((_, dest) => dest.Store.Guid = storeGuid));

            var result = await _storesService.NominateNewStoreManagerAsync(request.NominatingOwnershipGuid, nominatedOwnership);

            if (result)
            {
                return Ok();
            }

            return BadRequest();
        }

        private async Task<TReq> PopulateWithUserGuid<TReq>(TReq createRoleRequest) 
            where TReq : CreateRoleRequest
        {
            if (createRoleRequest.NominatedUserName == null)
            {
                return null;
            }
            var user = await _userService.GetBasicUserInfoAsync(createRoleRequest.NominatedUserName);
            if (user == null)
            {
                return null;
            }

            createRoleRequest.NominatedUserGuid = user.Guid;
            return createRoleRequest;
        }

        [Authorize]
        [HttpGet(ApiRoutes.Stores.Roles.Ownerships.SubordinatesGet)]
        public async Task<IActionResult> GetSubordinates(Guid ownershipGuid, [FromQuery] int? level = null)
        {
            var subordinates = await _storesService.GetSubordinateSellersAsync(ownershipGuid, level);
            if (subordinates == null)
            {
                return NotFound();
            }
            return Ok(_mapper.Map<StoreSellersResponse>(subordinates));
        }

        //[Authorize]
        [HttpPost("{storeGuid}/policy")]
        public async Task<IActionResult> CreatePolicy(Guid storeGuid, [FromBody] PolicyDto policy)
        {
            var createdPolicy = await _storesService.CreatePurchasePolicyAsync(storeGuid, policy);

            if (createdPolicy == null)
            {
                return NotFound();
            }
            var locationUrl = $"{this.GetBaseUrl()}/policies/{createdPolicy.Guid}";

            return Created(locationUrl, createdPolicy);
        }

        [Authorize]
        [HttpPost("{storeGuid}/policy/{policyGuid}/sub-policies")]
        public async Task<IActionResult> CreateSubPolicy(Guid storeGuid, Guid policyGuid, [FromBody] PolicyDto policy)
        {
            var createdPolicy = await _storesService.AddPolicyAsync(storeGuid, policyGuid, policy);

            if (createdPolicy == null)
            {
                return NotFound();
            }
            var locationUrl = $"{this.GetBaseUrl()}/policies/{createdPolicy.Guid}";

            return Created(locationUrl, createdPolicy);
        }

        [Authorize]
        [HttpDelete("{storeGuid}/policy/{policyGuid}")]
        public async Task<IActionResult> DeletePolicy(Guid storeGuid, Guid policyGuid)
        {
            var deletionResult = await _storesService.DeletePolicyAsync(storeGuid, policyGuid);

            if (!deletionResult)
            {
                return NotFound();
            }

            return NoContent();
        }
        [Authorize]
        [HttpGet("{storeGuid}/policy")]
        public async Task<IActionResult> GetPolicy(Guid storeGuid)
        {
            var policy = await _storesService.GetPolicyAsync(storeGuid);

            if (policy == null)
            {
                return NotFound();
            }

            return Ok(policy);
        }
    }
}
