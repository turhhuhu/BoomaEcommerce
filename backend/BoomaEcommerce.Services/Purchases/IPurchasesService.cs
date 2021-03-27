using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BoomaEcommerce.Domain;
using BoomaEcommerce.Services.DTO;

namespace BoomaEcommerce.Services.Purchases
{
    public interface IPurchasesService
    {
        /// <summary>
        /// Creates a product.
        /// </summary>
        /// <param name="purchase"></param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// </returns>
        Task CreatePurchaseAsync(PurchaseDto purchase);

        /// <summary>
        /// Gets all user purchase history.
        /// </summary>
        /// <param name="userId"></param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the purchase collection.
        /// </returns>
        Task<IReadOnlyCollection<PurchaseDto>> GetAllUserPurchaseHistoryAsync(string userId);

        /// <summary>
        /// Gets all store purchase history.
        /// </summary>
        /// <param name="storeGuid"></param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the purchase collection.
        /// </returns>
        Task<IReadOnlyCollection<PurchaseDto>> GetStorePurchaseHistoryAsync(Guid storeGuid);

        /// <summary>
        /// Get a product by guid.
        /// </summary>
        /// <param name="purchaseGuid"></param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the purchase.
        /// </returns>
        Task<PurchaseDto> GetPurchaseAsync(Guid purchaseGuid);

        /// <summary>
        /// Deletes a product by guid.
        /// </summary>
        /// <param name="purchaseGuid"></param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// </returns>
        Task DeletePurchaseAsync(Guid purchaseGuid);

        /// <summary>
        /// Updates a purchase by guid.
        /// </summary>
        /// <param name="purchase"></param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// </returns>
        Task UpdatePurchaseAsync(PurchaseDto purchase);
    }
}
