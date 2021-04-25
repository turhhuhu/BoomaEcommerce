using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BoomaEcommerce.Domain;
using BoomaEcommerce.Services.DTO;

namespace BoomaEcommerce.Services.Users
{
    public interface IUsersService
    {

        /// <summary>
        /// Gets shopping cart by userDto guid.
        /// </summary>
        /// <param name="userGuid"></param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the userDto's shopping cart.
        /// </returns>
        Task<ShoppingCartDto> GetShoppingCartAsync(Guid userGuid);

        /// <summary>
        /// Creates a shopping basket in a userDto's shopping cart.
        /// </summary>
        /// <param name="shoppingBasket"></param>
        /// <param name="shoppingCartGuid"></param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// </returns>
        Task<bool> CreateShoppingBasketAsync(Guid shoppingCartGuid, ShoppingBasketDto shoppingBasket);

        /// <summary>
        /// Adds a product to a userDto's shopping basket.
        /// </summary>
        /// <param name="shoppingBasketGuid"></param>
        /// <param name="purchaseProduct"></param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// </returns>
        Task<bool> AddPurchaseProductToShoppingBasketAsync(Guid shoppingBasketGuid, PurchaseProductDto purchaseProduct);

        /// <summary>
        /// Deletes a product from a userDto's shopping basket.
        /// </summary>
        /// <param name="shoppingBasketGuid"></param>
        /// <param name="purchaseProductGuid"></param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// </returns>
        Task<bool> DeletePurchaseProductFromShoppingBasketAsync(Guid shoppingBasketGuid, Guid purchaseProductGuid);

        /// <summary>
        /// Deletes a shopping basket in a userDto's shopping cart.
        /// </summary>
        /// <param name="shoppingBasketGuid"></param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// </returns>
        Task<bool> DeleteShoppingBasketAsync(Guid shoppingBasketGuid);

        /// <summary>
        /// Gets userDto info by id.
        /// </summary>
        /// <param name="userGuid"></param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the userDto's info.
        /// </returns>
        Task<UserDto> GetUserInfoAsync(Guid userGuid);

        /// <summary>
        /// Updates userDto info by id.
        /// </summary>
        /// <param name="userDto"></param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// </returns>
        Task UpdateUserInfoAsync(UserDto userDto);

    }
}
