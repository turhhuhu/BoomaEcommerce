using BoomaEcommerce.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using BoomaEcommerce.Services.DTO;

namespace BoomaEcommerce.Services.Stores
{
    public interface IStoresService
    {

        /// <summary>
        /// Gets the store's purchase history.
        /// </summary>
        /// <param name="storeGuid"></param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the store purchase history collection.
        /// </returns>
        Task<IReadOnlyCollection<StorePurchaseDto>> GetStorePurchaseHistory(Guid storeGuid);

        /// <summary>
        /// Creates a store for a registered user.
        /// </summary>
        /// <param name="store"></param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// </returns>
        Task<StoreDto> CreateStoreAsync(StoreDto store);

        /// <summary>
        /// Creates a product in a store.
        /// </summary>
        /// <param name="productDto"></param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// </returns>
        Task<ProductDto> CreateStoreProductAsync(ProductDto productDto);

        /// <summary>
        /// Deletes a product by guid.
        /// </summary>
        /// <param name="productGuid"></param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// </returns>
        Task<bool> DeleteProductAsync(Guid productGuid);

        /// <summary>
        /// Updates a product by guid.
        /// </summary>
        /// <param name="product"></param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// </returns>
        Task<bool> UpdateProductAsync(ProductDto product);

        /// <summary>
        /// Gets all stores.
        /// </summary>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the stores collection.
        /// </returns>
        Task<IReadOnlyCollection<StoreDto>> GetStoresAsync();

        /// <summary>
        /// Gets a store by guid.
        /// </summary>
        /// <param name="storeGuid"></param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the store.
        /// </returns>
        Task<StoreDto> GetStoreAsync(Guid storeGuid);

        /// <summary>
        /// Deletes a store by guid.
        /// </summary>
        /// <param name="storeGuid"></param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// </returns>
        Task<bool> DeleteStoreAsync(Guid storeGuid);

        /// <summary>
        /// Adds a new manager to a store
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
        /// <param name="manager"></param>
        /// <param name="newManagementDto"></param>
        /// <returns>
        /// return bool that represents if the nomination process was successful
        /// </returns>
        Task<bool> NominateNewStoreManager(Guid manager, StoreManagementDto newManagementDto);

        /// <summary>
        /// Get of all subordinates under the store manager provided
        /// </summary>
        /// <param name="storeOwnerGuid"></param>
        /// <returns>
        /// A task that represents the asynchronous operation.
        /// returns a store seller response containing all subordinates of the manager requested.
        /// </returns>
        Task<StoreSellersResponse> GetAllSubordinateSellers(Guid storeOwnerGuid);
        
        Task<StoreOwnershipDto> GetStoreOwnerShip(Guid userGuid, Guid storeGuid);
        Task<StoreManagementDto> GetStoreManagement(Guid userGuid, Guid storeGuid);
        Task<ProductDto> GetStoreProduct(Guid productGuid);

        Task<StoreSellersResponse> GetAllSellersInformation(Guid storeGuid);

        Task<IReadOnlyCollection<StoreOwnershipDto>> GetAllStoreOwnerShips(Guid userGuid);
        
        Task<IReadOnlyCollection<StoreManagementDto>> GetAllStoreManagements(Guid userGuid);
        
        /// <summary>
        /// Removing a manager that owner nominated
        /// </summary>
        /// <param name="removeOwner"></param>
        /// <param name="removeManager"></param>
        /// <returns>
        /// returns true if succeed else false
        /// </returns>
        Task<Boolean> RemoveManager(Guid removeOwnership, Guid removeManagement);

    }
}
