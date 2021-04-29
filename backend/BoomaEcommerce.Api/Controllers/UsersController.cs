using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BoomaEcommerce.Core;
using BoomaEcommerce.Domain;
using BoomaEcommerce.Services.DTO;
using BoomaEcommerce.Services.Stores;
using BoomaEcommerce.Services.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;

namespace BoomaEcommerce.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly IUsersService _userService;
        private readonly IStoresService _storesService;

        public UsersController(IUsersService userService, IStoresService storesService)
        {
            _userService = userService;
            _storesService = storesService;
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
        public async Task<IActionResult> PostBasket([FromBody] ShoppingBasketDto shoppingBasket)
        {
            var userGuid = User.GetUserGuid();
            var createdBasket = await _userService.CreateShoppingBasketAsync(userGuid, shoppingBasket);
            if (createdBasket == null)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }

            return Ok(createdBasket);
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
        public async Task<IActionResult> PostPurchaseProduct(Guid basketGuid, [FromBody] PurchaseProductDto purchaseProduct)
        {
            var res =
                await _userService.AddPurchaseProductToShoppingBasketAsync(basketGuid, purchaseProduct);
            if (res == null)
            {
                return NotFound();
            }

            return Ok(res);
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

        [HttpPost(ApiRoutes.Store.MePost)]
        public async Task<IActionResult> CreateStore([FromBody] StoreDto store)
        {
            var storeResult = await _storesService.CreateStoreAsync(store);

            if (storeResult == null)
            {
                return BadRequest();
            }

            return Ok(storeResult);
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
    }
}
