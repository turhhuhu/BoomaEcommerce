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
        /// <param name="userId"></param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the user's shopping cart.
        /// </returns>
        Task<ShoppingCartDto> GetShoppingCartAsync(string userId);

        /// <summary>
        /// Creates a shopping basket in a user's shopping cart.
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="shoppingBasket"></param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// </returns>
        Task CreateShoppingBasketAsync(string userId, ShoppingBasketDto shoppingBasket);

        /// <summary>
        /// Adds a product to a user's shopping basket.
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="purchaseProduct"></param>
        /// <param name="storeGuid"></param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// </returns>
        Task AddProductToShoppingBasketAsync(string userId, Guid storeGuid, PurchaseProductDto purchaseProduct);

        /// <summary>
        /// Deletes a product from a user's shopping basket.
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="purchaseProduct"></param>
        /// <param name="shoppingBasket"></param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// </returns>
        Task DeleteProductFromShoppingBasketAsync(string userId, PurchaseProductDto purchaseProduct, ShoppingBasketDto shoppingBasket);

        /// <summary>
        /// Deletes a shopping basket in a user's shopping cart.
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="storeGuid"></param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// </returns>
        Task DeleteShoppingBasketAsync(string userId, Guid storeGuid);

        /// <summary>
        /// Gets user info by id.
        /// </summary>
        /// <param name="userId"></param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the user's info.
        /// </returns>
        Task<UserDto> GetUserInfoAsync(string userId);

        /// <summary>
        /// Updates user info by id.
        /// </summary>
        /// <param name="user"></param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// </returns>
        Task UpdateUserInfoAsync(UserDto user);

        /// <summary>
        /// Adds a new owner to a store
        /// </summary>
        /// <param name="owner"></param>
        /// <param name="newOwnerDto"></param>
        /// <returns>
        /// return bool that represents if the nomination process was successful
        /// </returns>
        Task<bool> NominateNewStoreOwner(Guid owner, StoreOwnershipDto newOwnerDto);
        
        /// <summary>
        /// Adds a new manager to a store
        /// </summary>
        /// <param name="owner"></param>
        /// <param name="newOwnerDto"></param>
        /// <returns>
        /// return bool that represents if the nomination process was successful
        /// </returns>
        Task<bool> NominateNewStoreManager(Guid owner, StoreManagementDto newOwnerDto);

    }
}
