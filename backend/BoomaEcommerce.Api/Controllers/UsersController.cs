using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using BoomaEcommerce.Api.Hubs;
using BoomaEcommerce.Api.Responses;
using BoomaEcommerce.Core;
using BoomaEcommerce.Core.Exceptions;
using BoomaEcommerce.Domain;
using BoomaEcommerce.Services.DTO;
using BoomaEcommerce.Services.Stores;
using BoomaEcommerce.Services.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using BoomaEcommerce.Services;
using BoomaEcommerce.Services.DTO.ProductOffer;
using BoomaEcommerce.Services.Purchases;
using Microsoft.AspNetCore.SignalR;

namespace BoomaEcommerce.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly IUsersService _userService;
        private readonly IStoresService _storesService;
        private readonly IPurchasesService _purchaseService;
        private readonly INotificationPublisher _notificationPublisher;
        private readonly IMapper _mapper;


        public UsersController(IUsersService userService,
            IStoresService storesService,
            IPurchasesService purchaseService,
            INotificationPublisher notificationPublisher,
            IMapper mapper)
        {
            _userService = userService;
            _storesService = storesService;
            _purchaseService = purchaseService;
            _notificationPublisher = notificationPublisher;
            _mapper = mapper;
        }

        [HttpPost(ApiRoutes.Notifications.Post)]
        public async Task<IActionResult> PostNotification(Guid userGuid, NotificationDto notf)
        {
            //Generate fake guid only for frontend 
            notf.Guid = Guid.NewGuid();
            await _notificationPublisher.NotifyAsync(notf, userGuid);
            return Ok();
        }

        [Authorize]
        [HttpGet(ApiRoutes.Me)]
        public async Task<IActionResult> GetUserInfo()
        {
            var userGuid = User.GetUserGuid();
            var userInfo = await _userService.GetUserInfoAsync(userGuid);

            if (userInfo == null)
            {
                return NotFound();
            }
            return Ok(userInfo);
        }

        [Authorize]
        [HttpPost(ApiRoutes.Cart.Baskets.MePost)]
        public async Task<IActionResult> CreateBasket([FromBody] ShoppingBasketDto shoppingBasket)
        {
            var userGuid = User.GetUserGuid();
            var createdBasket = await _userService.CreateShoppingBasketAsync(userGuid, shoppingBasket);
            if (createdBasket == null)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
            return CreatedAtAction(nameof(CreateBasket), createdBasket);

        }

        [Authorize]
        [HttpDelete(ApiRoutes.Cart.Baskets.MeDelete)]
        public async Task<IActionResult> DeleteBasket(Guid basketGuid)
        {
            var result = await _userService.DeleteShoppingBasketAsync(basketGuid);
            if (result)
            {
                return NoContent();
            }

            return NotFound();
        }

        [Authorize]
        [HttpPost(ApiRoutes.Cart.Baskets.PurchaseProducts.MePost)]
        public async Task<IActionResult> CreatePurchaseProduct(Guid basketGuid, [FromBody] PurchaseProductDto purchaseProduct)
        {
            var res =
                await _userService.AddPurchaseProductToShoppingBasketAsync(User.GetUserGuid(), basketGuid, purchaseProduct);
            if (res == null)
            {
                return NotFound();
            }
            return CreatedAtAction(nameof(CreatePurchaseProduct), res);
        }

        [Authorize]
        [HttpDelete(ApiRoutes.Cart.Baskets.PurchaseProducts.MeDelete)]
        public async Task<IActionResult> DeletePurchaseProduct(Guid basketGuid, Guid purchaseProductGuid)
        {
            var res = await _userService.DeletePurchaseProductFromShoppingBasketAsync(basketGuid, purchaseProductGuid);
            if (res)
            {
                return NoContent();
            }
            
            return NotFound();
        }

        [HttpPost(ApiRoutes.Stores.MePost)]
        public async Task<IActionResult> CreateStore([FromBody] StoreDto store)
        {
            
            var storeResult = await _storesService.CreateStoreAsync(store);

            if (storeResult == null)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
            var baseUrl = this.GetBaseUrl();
            var locationUrl = $"{baseUrl}/stores/{storeResult.Guid}";

            return Created(locationUrl, storeResult);
        }
        [Authorize]
        [HttpGet(ApiRoutes.Cart.MeGet)]
        public async Task<IActionResult> GetCart()
        {
            var userGuid = User.GetUserGuid();
            var cart = await _userService.GetShoppingCartAsync(userGuid);
            if (cart == null)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }

            return Ok(cart);
        }

        [Authorize]
        [HttpGet(ApiRoutes.Stores.AllRolesGet)]
        public async Task<IActionResult> GetStoreRoles()
        {
            var userGuid = User.GetUserGuid();
            var ownerships = await _storesService.GetAllStoreOwnerShipsAsync(userGuid);
            var managements = await _storesService.GetAllStoreManagementsAsync(userGuid);

            if (ownerships == null || managements == null)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }

            var (founderRoles, ownershipRoles) =
                ownerships.Split(ownership => ownership.Store.FounderUserGuid == userGuid);

            var roles = new StoreRolesResponse
            {
                OwnerFounderRoles = _mapper.Map<IEnumerable<OwnerShipRoleResponse>>(founderRoles),
                OwnerNotFounderRoles = _mapper.Map<IEnumerable<OwnerShipRoleResponse>>(ownershipRoles),
                ManagerRoles = _mapper.Map<IEnumerable<ManagementRoleResponse>>(managements)
            };
            return Ok(roles);
        }
        
        [Authorize]
        [HttpGet(ApiRoutes.Stores.Roles.MeRoleGet)]
        public async Task<IActionResult> GetRole(Guid storeGuid)
        {
            var userGuid = User.GetUserGuid();

            var management = await _storesService.GetStoreManagementAsync(userGuid, storeGuid);
            if (management != null)
            {
                return Ok(_mapper.Map<ManagementRoleResponse>(management));
            }

            try
            {
                var ownership = await _storesService.GetStoreOwnerShipAsync(userGuid, storeGuid);
                if (ownership != null)
                {
                    return Ok(_mapper.Map<OwnerShipRoleResponse>(ownership));
                }
            }
            catch (UnAuthorizedException)
            {
            }
            return NotFound();

        }


        [Authorize]
        [HttpGet(ApiRoutes.Purchases.Get)]
        public async Task<IActionResult> GetPurchaseHistory()
        {
            var userGuid = User.GetUserGuid();
            var history = await _purchaseService.GetAllUserPurchaseHistoryAsync(userGuid);
            if (history == null)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }

            return Ok(history);
        }

        [Authorize]
        [HttpPut(ApiRoutes.Notifications.PutSeen)]
        public async Task<IActionResult> PutSeenNotification(Guid notificationGuid)
        {
            var isUpdated = await _userService.SetNotificationAsSeen(User.GetUserGuid(), notificationGuid);
            if (isUpdated)
            {
                return NoContent();
            }

            return NotFound();
        }

        [Authorize]
        [HttpPost(ApiRoutes.Me + "/offers")]
        public async Task<IActionResult> CreateProductOffer(ProductOfferDto offerDto)
        {
            var createdProductOfferDto = await _userService.CreateProductOffer(offerDto);
            if (createdProductOfferDto == null)
            {
                return NotFound();
            }

            return Ok(createdProductOfferDto);
        }

        [Authorize]
        [HttpGet(ApiRoutes.Me + "/offers")]
        public async Task<IActionResult> GetUserProductOffers()
        {
            var userGuid = User.GetUserGuid();
            var offers = await _storesService.GetAllUserProductOffers(userGuid);
            if (offers == null)
            {
                return NotFound();
            }

            return Ok(offers.ToList());
        }
    }
}
