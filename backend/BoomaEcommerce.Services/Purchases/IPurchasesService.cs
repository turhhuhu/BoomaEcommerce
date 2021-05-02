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
        Task<bool> CreatePurchaseAsync(PurchaseDto purchase);

        /// <summary>
        /// Gets all user purchase history.
        /// </summary>
        /// <param name="userId"></param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the purchase collection.
        /// </returns>
        Task<IReadOnlyCollection<PurchaseDto>> GetAllUserPurchaseHistoryAsync(Guid userId);


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
