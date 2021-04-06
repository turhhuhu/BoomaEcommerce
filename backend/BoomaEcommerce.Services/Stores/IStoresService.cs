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
        /// Creates a store for a registered user.
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="store"></param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// </returns>
        Task CreateStoreAsync(string userId, StoreDto store);

        /// <summary>
        /// Gets all stores.
        /// </summary>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the stores collection.
        /// </returns>
        Task<IReadOnlyCollection<StoreDto>> GetAllStoresAsync();

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
        Task DeleteStoreAsync(Guid storeGuid);
    }
}
