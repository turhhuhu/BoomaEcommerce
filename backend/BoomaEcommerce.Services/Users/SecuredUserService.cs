using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using BoomaEcommerce.Core;
using BoomaEcommerce.Core.Exceptions;
using BoomaEcommerce.Services.DTO;
using BoomaEcommerce.Services.Stores;
using UnAuthorizedException = BoomaEcommerce.Services.Exceptions.UnAuthorizedException;

namespace BoomaEcommerce.Services.Users
{
    public class SecuredUserService : SecuredServiceBase, IUsersService
    {
        private readonly IUsersService _next;
        public static bool CreateSecuredStoreService(string token, string secret, IUsersService next, out IUsersService userService)
        {
            try
            {
                var claimsPrincipal = ValidateToken(token, secret);
                userService = new SecuredUserService(claimsPrincipal, next);
                return true;
            }
            catch (Exception e)
            {
                userService = null;
                return false;
            }
        }
        protected SecuredUserService(ClaimsPrincipal claimsPrincipal, IUsersService next) : base(claimsPrincipal)
        {
            _next = next;
        }
        public Task<ShoppingCartDto> GetShoppingCartAsync(Guid userGuid)
        {
            CheckAuthenticated();
            var userGuidInToken = ClaimsPrincipal.GetUserGuid();

            if (userGuid == userGuidInToken)
            {
                return _next.GetShoppingCartAsync(userGuid);
            }

            throw new UnAuthorizedException($"User {userGuidInToken} can not access {userGuid} shopping cart.");
        }

        public async Task<bool> CreateShoppingBasketAsync(Guid shoppingCartGuid, ShoppingBasketDto shoppingBasket)
        {
            CheckAuthenticated();

            var userGuidInToken = ClaimsPrincipal.GetUserGuid();

            var shoppingCart = await _next.GetShoppingCartAsync(userGuidInToken);

            if (shoppingCart.Guid == shoppingCartGuid)
            {
                return await _next.CreateShoppingBasketAsync(shoppingCartGuid, shoppingBasket);
            }

            throw new UnAuthorizedException($"User {userGuidInToken} can not add a basket to {shoppingCartGuid} shopping cart.");
        }

        public async Task<bool> AddPurchaseProductToShoppingBasketAsync(Guid shoppingBasketGuid, PurchaseProductDto purchaseProduct)
        {
            CheckAuthenticated();

            var userGuidInToken = ClaimsPrincipal.GetUserGuid();

            var shoppingCart = await _next.GetShoppingCartAsync(userGuidInToken);

            if (shoppingCart.Baskets.ContainsKey(shoppingBasketGuid))
            {
                return await _next.AddPurchaseProductToShoppingBasketAsync(shoppingBasketGuid, purchaseProduct);
            }

            throw new UnAuthorizedException($"User {userGuidInToken} cannot add a purchase product to {shoppingBasketGuid} shopping basket.");
        }

        public async Task<bool> DeletePurchaseProductFromShoppingBasketAsync(Guid shoppingBasketGuid, Guid purchaseProductGuid)
        {
            CheckAuthenticated();
            var userGuidInToken = ClaimsPrincipal.GetUserGuid();

            var shoppingCart = await _next.GetShoppingCartAsync(userGuidInToken);

            if (shoppingCart.Baskets.ContainsKey(shoppingBasketGuid))
            {
                return await _next.DeletePurchaseProductFromShoppingBasketAsync(shoppingBasketGuid, purchaseProductGuid);
            }

            throw new UnAuthorizedException($"User {userGuidInToken} cannot remove a purchase product from {shoppingBasketGuid} shopping basket.");
        }

        public async Task<bool> DeleteShoppingBasketAsync(Guid shoppingBasketGuid)
        {
            CheckAuthenticated();

            var userGuidInToken = ClaimsPrincipal.GetUserGuid();

            var shoppingCart = await _next.GetShoppingCartAsync(userGuidInToken);

            if (shoppingCart.Baskets.ContainsKey(shoppingBasketGuid))
            {
                return await _next.DeleteShoppingBasketAsync(shoppingBasketGuid);
            }

            throw new UnAuthorizedException($"User {userGuidInToken} can not delete a basket from {shoppingCart.Guid} shopping cart.");
        }

        public Task<UserDto> GetUserInfoAsync(Guid userGuid)
        {
            CheckAuthenticated();

            var userGuidInToken = ClaimsPrincipal.GetUserGuid();
            if (userGuidInToken == userGuid)
            {
                return _next.GetUserInfoAsync(userGuid);
            }

            throw new UnAuthorizedException($"User {userGuidInToken} can not get user information of user {userGuid}");
        }

        public Task UpdateUserInfoAsync(UserDto user)
        {
            CheckAuthenticated();

            var userGuidInToken = ClaimsPrincipal.GetUserGuid();
            if (userGuidInToken == user.Guid)
            {
                return _next.UpdateUserInfoAsync(user);
            }

            throw new UnAuthorizedException($"User {userGuidInToken} can not update user information of user {user.Guid}");
        }

    }
}
