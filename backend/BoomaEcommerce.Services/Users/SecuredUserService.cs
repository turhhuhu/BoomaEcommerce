using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using BoomaEcommerce.Core;
using BoomaEcommerce.Services.DTO;
using BoomaEcommerce.Services.Stores;
using UnAuthorizedException = BoomaEcommerce.Core.Exceptions.UnAuthorizedException;

namespace BoomaEcommerce.Services.Users
{
    public class SecuredUserService : SecuredServiceBase, IUsersService
    {
        private readonly IUsersService _next;

        public SecuredUserService(IUsersService next)
        {
            _next = next;
        }
        public SecuredUserService(ClaimsPrincipal claimsPrincipal, IUsersService next) : base(claimsPrincipal)
        {
            _next = next;
        }
        public static bool CreateSecuredUserService(string token, string secret, IUsersService next, out IUsersService userService)
        {
            try
            {
                var claimsPrincipal = ValidateToken(token, secret);
                userService = new SecuredUserService(claimsPrincipal, next);
                return true;
            }
            catch
            {
                userService = null;
                return false;
            }
        }
        public async Task<ShoppingCartDto> GetShoppingCartAsync(Guid userGuid)
        {
            CheckAuthenticated();
            var userGuidInToken = ClaimsPrincipal.GetUserGuid();
            var userInfo = await _next.GetUserInfoAsync(userGuid);
            if (userInfo != null && userGuid == userGuidInToken)
            {
                return await _next.GetShoppingCartAsync(userGuid);
            }

            throw new UnAuthorizedException($"User {userGuidInToken} can not access {userGuid} shopping cart.");
        }

        public async Task<ShoppingBasketDto> CreateShoppingBasketAsync(Guid shoppingCartGuid, ShoppingBasketDto shoppingBasket)
        {
            ServiceUtilities.ValidateDto<ShoppingBasketDto, UserServiceValidators.CreateShoppingBasketAsync>(shoppingBasket);
            
            CheckAuthenticated();

            var userGuidInToken = ClaimsPrincipal.GetUserGuid();

            var shoppingCart = await _next.GetShoppingCartAsync(userGuidInToken);

            if (shoppingCart.Guid == shoppingCartGuid)
            {
                return await _next.CreateShoppingBasketAsync(shoppingCartGuid, shoppingBasket);
            }

            throw new UnAuthorizedException($"User {userGuidInToken} can not add a basket to {shoppingCartGuid} shopping cart.");
        }

        public async Task<PurchaseProductDto> AddPurchaseProductToShoppingBasketAsync(Guid userGuid, Guid shoppingBasketGuid,
            PurchaseProductDto purchaseProduct)
        {
            ServiceUtilities.ValidateDto<PurchaseProductDto, UserServiceValidators.AddPurchaseProductToShoppingBasketAsync>(purchaseProduct);
            CheckAuthenticated();

            var userGuidInToken = ClaimsPrincipal.GetUserGuid();

            var shoppingCart = await _next.GetShoppingCartAsync(userGuidInToken);

            if (shoppingCart.Baskets.FirstOrDefault(basket => basket.Guid == shoppingBasketGuid) != null)
            {
                return await _next.AddPurchaseProductToShoppingBasketAsync(userGuid, shoppingBasketGuid, purchaseProduct);
            }
            throw new UnAuthorizedException($"User {userGuidInToken} cannot add a purchase product to {shoppingBasketGuid} shopping basket.");
        }

        public async Task<bool> DeletePurchaseProductFromShoppingBasketAsync(Guid shoppingBasketGuid, Guid purchaseProductGuid)
        {
            CheckAuthenticated();
            var userGuidInToken = ClaimsPrincipal.GetUserGuid();

            var shoppingCart = await _next.GetShoppingCartAsync(userGuidInToken);

            if (shoppingCart?.Baskets.FirstOrDefault(basket => basket.Guid == shoppingBasketGuid) != null)
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

            if (shoppingCart.Baskets.FirstOrDefault(basket => basket.Guid == shoppingBasketGuid) != null)
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

            throw new UnAuthorizedException($"User {userGuidInToken} can not get userDto information of userDto {userGuid}");
        }

        public Task<bool> UpdateUserInfoAsync(UserDto userDto)
        {
            ServiceUtilities.ValidateDto<UserDto, UserServiceValidators.UpdateUserInfoAsync>(userDto);
            CheckAuthenticated();

            var userGuidInToken = ClaimsPrincipal.GetUserGuid();
            if (userGuidInToken == userDto.Guid)
            {
                return _next.UpdateUserInfoAsync(userDto);
            }

            throw new UnAuthorizedException($"User {userGuidInToken} can not update userDto information of userDto {userDto.Guid}");
        }

        public Task<BasicUserInfoDto> GetBasicUserInfoAsync(string userName)
        {
            return _next.GetBasicUserInfoAsync(userName);
        }

        public Task<bool> SetNotificationAsSeen(Guid userGuid, Guid notificationGuid)
        {
            var userInClaims = ClaimsPrincipal.GetUserGuid();
            if (userGuid == userInClaims)
            {
                return _next.SetNotificationAsSeen(userGuid, notificationGuid);
            }

            throw new UnAuthorizedException(
                $"User with guid {userInClaims} is not authorized to set notifications of user with guid {userGuid}");
        }
    }
}
