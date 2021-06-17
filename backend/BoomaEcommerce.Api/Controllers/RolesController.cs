using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using System.Threading.Tasks;
using AutoMapper;
using BoomaEcommerce.Api.Responses;
using BoomaEcommerce.Services.DTO;
using BoomaEcommerce.Services.Stores;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BoomaEcommerce.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RolesController : ControllerBase
    {
        private readonly IStoresService _storesService;
        private readonly IMapper _mapper;

        public RolesController(IStoresService storesService, IMapper mapper)
        {
            _storesService = storesService;
            _mapper = mapper;
        }

        [Authorize]
        [HttpGet(ApiRoutes.Roles.Ownerships.Get)]
        public async Task<IActionResult> GetOwnership(Guid ownershipGuid)
        {
            var ownership = await _storesService.GetStoreOwnershipAsync(ownershipGuid);
            if (ownership == null)
            {
                return NotFound();
            }

            return Ok(_mapper.Map<OwnerShipRoleResponse>(ownership));
        }

        [Authorize]
        [HttpGet(ApiRoutes.Roles.Managements.Get)]
        public async Task<IActionResult> GetManagement(Guid managementGuid)
        {
            var management = await _storesService.GetStoreManagementAsync(managementGuid);
            if (management == null)
            {
                return NotFound();
            }

            return Ok(_mapper.Map<ManagementRoleResponse>(management));
        }

        [Authorize]
        [HttpPut(ApiRoutes.Roles.Managements.PutPermissions)]
        public async Task<IActionResult> UpdatePermissions(Guid managementGuid , [FromBody] StoreManagementPermissionsDto permissions)
        {
            permissions.Guid = managementGuid;
            await _storesService.UpdateManagerPermissionAsync(permissions);
            return NoContent();
        }

        [Authorize]
        [HttpGet(ApiRoutes.Roles.Ownerships.GetSubordinates)]
        public async Task<IActionResult> GetSubordinates(Guid ownershipGuid, [FromQuery] int? level = null)
        {
            var subordinates = await _storesService.GetSubordinateSellersAsync(ownershipGuid, level);
            if (subordinates == null)
            {
                return NotFound();
            }

            return Ok(_mapper.Map<StoreSellersResponse>(subordinates));
        }

        [Authorize]
        [HttpDelete(ApiRoutes.Roles.Ownerships.DeleteSubordinate)]
        public async Task<IActionResult> DeleteRole(Guid ownershipGuid, Guid roleToDeleteGuid, [FromQuery] RoleType roleType)
        {
            bool isDeleted;
            if (roleType == RoleType.Ownership)
            {
                isDeleted = await _storesService.RemoveStoreOwnerAsync(ownershipGuid, roleToDeleteGuid);
            }
            else
            {
                isDeleted = await _storesService.RemoveManagerAsync(ownershipGuid, roleToDeleteGuid);
            }

            if (isDeleted)
            {
                return NoContent();
            }

            return NotFound();
        }

        [Authorize]
        [HttpGet(ApiRoutes.Roles.Ownerships.OwnershipGuid + "/offers")]
        public async Task<IActionResult> GetAllOffersOwner(Guid ownershipGuid)
        {
            var offers = await _storesService.GetAllOwnerProductOffers(ownershipGuid);
            if (offers == null)
            {
                return NotFound();
            }

            return Ok(offers.ToList());
        }

        [Authorize]
        [HttpPost(ApiRoutes.Roles.Ownerships.OwnershipGuid + "/offers/{offerGuid}/approve")]
        public async Task<IActionResult> ApproveProductOffer(Guid ownershipGuid, Guid offerGuid)
        {
            await _storesService.ApproveOffer(ownershipGuid, offerGuid);
            return Ok();
        }

        [Authorize]
        [HttpPost(ApiRoutes.Roles.Ownerships.OwnershipGuid + "/offers/{offerGuid}/decline")]
        public async Task<IActionResult> DeclineProductOffer(Guid ownershipGuid, Guid offerGuid)
        {
            await _storesService.DeclineOffer(ownershipGuid, offerGuid);
            return Ok();
        }

        [Authorize]
        [HttpPost(ApiRoutes.Roles.Ownerships.OwnershipGuid + "/offers/{offerGuid}/counterOffer")]
        public async Task<IActionResult> ProposeCounterOffer(Guid ownershipGuid, Guid offerGuid, decimal counterOfferPrice)
        {
            var offerDto = await _storesService.MakeCounterOffer(ownershipGuid, counterOfferPrice, offerGuid);
            if (offerDto == null)
            {
                return NotFound();
            }

            return Ok(offerDto);
        }
    }
}
