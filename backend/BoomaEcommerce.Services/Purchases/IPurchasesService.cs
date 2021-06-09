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
        /// <param name="purchaseDetailsDto"></param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// </returns>
        Task<PurchaseDto> CreatePurchaseAsync(PurchaseDetailsDto purchaseDetailsDto);

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
        /// Gets the purchase final price after discount calculation
        /// </summary>
        /// <param name="purchaseDto"></param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the purchase final price
        /// </returns>
        Task<decimal> GetPurchaseFinalPrice(PurchaseDto purchaseDto);


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
