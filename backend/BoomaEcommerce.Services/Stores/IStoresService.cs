using BoomaEcommerce.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
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
        Task CreateStoreAsync(StoreDto store);

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

        /// <summary>
        /// Get of all subordinates under the store owner provided
        /// </summary>
        /// <param name="storeOwnerGuid"></param>
        /// <returns>
        /// A task that represents the asynchronous operation.
        /// returns a store seller response containing all subordinates of the owner requested.
        /// </returns>
        Task<StoreSellersResponse> GetAllSubordinateSellers(Guid storeOwnerGuid);
    }
}
