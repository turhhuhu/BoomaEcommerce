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
        /// Gets shopping cart by user guid.
        /// </summary>
        /// <param name="userGuid"></param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the user's shopping cart.
        /// </returns>
        Task<ShoppingCartDto> GetShoppingCartAsync(Guid userGuid);

        /// <summary>
        /// Creates a shopping basket in a user's shopping cart.
        /// </summary>
        /// <param name="shoppingBasket"></param>
        /// <param name="shoppingCartGuid"></param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// </returns>
        Task CreateShoppingBasketAsync(Guid shoppingCartGuid, ShoppingBasketDto shoppingBasket);

        /// <summary>
        /// Adds a product to a user's shopping basket.
        /// </summary>
        /// <param name="shoppingBasketGuid"></param>
        /// <param name="purchaseProduct"></param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// </returns>
        Task AddProductToShoppingBasketAsync(Guid shoppingBasketGuid, PurchaseProductDto purchaseProduct);

        /// <summary>
        /// Deletes a product from a user's shopping basket.
        /// </summary>
        /// <param name="shoppingBasketGuid"></param>
        /// <param name="purchaseProductGuid"></param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// </returns>
        Task DeleteProductFromShoppingBasketAsync(Guid shoppingBasketGuid, Guid purchaseProductGuid);

        /// <summary>
        /// Deletes a shopping basket in a user's shopping cart.
        /// </summary>
        /// <param name="shoppingBasketGuid"></param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// </returns>
        Task DeleteShoppingBasketAsync(Guid shoppingBasketGuid);

        /// <summary>
        /// Gets user info by id.
        /// </summary>
        /// <param name="userGuid"></param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the user's info.
        /// </returns>
        Task<UserDto> GetUserInfoAsync(Guid userGuid);

        /// <summary>
        /// Updates user info by id.
        /// </summary>
        /// <param name="user"></param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// </returns>
        Task UpdateUserInfoAsync(UserDto user);

    }
}
