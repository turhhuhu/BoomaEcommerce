using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BoomaEcommerce.Core;
using BoomaEcommerce.Domain;
using BoomaEcommerce.Services.DTO;
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

        public UsersController(IUsersService userService)
        {
            _userService = userService;
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
        [HttpPost(ApiRoutes.Cart.Baskets.PostMe)]
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
        [HttpPost(ApiRoutes.Cart.Baskets.PurchaseProducts.Post)]
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
        [HttpGet(ApiRoutes.Cart.GetMe)]
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
