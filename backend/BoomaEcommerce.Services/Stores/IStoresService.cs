using BoomaEcommerce.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using BoomaEcommerce.Services.DTO;
using BoomaEcommerce.Services.DTO.Discounts;
using BoomaEcommerce.Services.DTO.Policies;
using BoomaEcommerce.Services.DTO.ProductOffer;

namespace BoomaEcommerce.Services.Stores
{
    public interface IStoresService
    {

        /// <summary>
        /// Gets the UserStore's purchase history.
        /// </summary>
        /// <param name="storeGuid"></param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the UserStore purchase history collection.
        /// </returns>
        Task<IReadOnlyCollection<StorePurchaseDto>> GetStorePurchaseHistoryAsync(Guid storeGuid);

        /// <summary>
        /// Creates a UserStore for a registered user.
        /// </summary>
        /// <param name="store"></param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// </returns>
        Task<StoreDto> CreateStoreAsync(StoreDto store);

        /// <summary>
        /// Creates a product in a UserStore.
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
        /// Gets a UserStore by guid.
        /// </summary>
        /// <param name="storeGuid"></param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the UserStore.
        /// </returns>
        Task<StoreDto> GetStoreAsync(Guid storeGuid);

        /// <summary>
        /// Deletes a UserStore by guid.
        /// </summary>
        /// <param name="storeGuid"></param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// </returns>
        Task<bool> DeleteStoreAsync(Guid storeGuid);

        /// <summary>
        /// Adds a new ownerGuid to a UserStore
        /// </summary>
        /// <param name="ownerGuid"></param>
        /// <param name="newOwnerDto"></param>
        /// <returns>
        /// return bool that represents if the nomination process was successful
        /// </returns>
        Task<bool> NominateNewStoreOwnerAsync(Guid ownerGuid, StoreOwnershipDto newOwnerDto);

        /// <summary>
        /// Adds a new ownerGuid to a UserStore
        /// </summary>
        /// <param name="ownerGuid"></param>
        /// <param name="newManagementDto"></param>
        /// <returns>
        /// return bool that represents if the nomination process was successful
        /// </returns>
        Task<bool> NominateNewStoreManagerAsync(Guid ownerGuid, StoreManagementDto newManagementDto);

        /// <summary>
        /// Get of all subordinates under the UserStore ownerGuid provided
        /// </summary>
        /// <param name="storeOwnerGuid"></param>
        /// <param name="level"></param>
        /// <returns>
        /// A task that represents the asynchronous operation.
        /// returns a UserStore seller response containing all subordinates of the ownerGuid requested.
        /// </returns>
        Task<StoreSellersDto> GetSubordinateSellersAsync(Guid storeOwnerGuid, int? level = null);
        
        Task<StoreOwnershipDto> GetStoreOwnerShipAsync(Guid userGuid, Guid storeGuid);
        Task<StoreManagementDto> GetStoreManagementAsync(Guid userGuid, Guid storeGuid);
        Task<ProductDto> GetStoreProductAsync(Guid productGuid);

        Task<StoreSellersDto> GetAllSellersInformationAsync(Guid storeGuid);

        Task<IReadOnlyCollection<StoreOwnershipDto>> GetAllStoreOwnerShipsAsync(Guid userGuid);
        
        Task<IReadOnlyCollection<StoreManagementDto>> GetAllStoreManagementsAsync(Guid userGuid);
      
        Task<IReadOnlyCollection<ProductDto>> GetProductsFromStoreAsync(Guid storeGuid);

        Task<bool> RemoveStoreOwnerAsync(Guid ownerGuidRemoveFrom, Guid ownerGuidToRemove);

        Task UpdateManagerPermissionAsync(StoreManagementPermissionsDto smpDto);

        Task<StoreManagementDto> GetStoreManagementAsync(Guid storeManagementGuid);

        Task<StoreOwnershipDto> GetStoreOwnershipAsync(Guid storeOwnershipGuid);

        /// <summary>
        /// Removing a ownerGuid that ownerGuid nominated
        /// </summary>
        /// <param name="removeOwner"></param>
        /// <param name="removeManager"></param>
        /// <returns>
        /// returns true if succeed else false
        /// </returns>
        Task<bool> RemoveManagerAsync(Guid ownershipToRemoveFrom, Guid managerToRemove);

        Task<PolicyDto> AddPolicyAsync(Guid storeGuid, Guid policyGuid, PolicyDto childPolicyDto);
        Task<bool> DeletePolicyAsync(Guid storeGuid, Guid policyGuid);
        Task<PolicyDto> CreatePurchasePolicyAsync(Guid storeGuid, PolicyDto policyDto);
        Task<PolicyDto> GetPolicyAsync(Guid storeGuid);

        Task<DiscountDto> AddDiscountAsync(Guid storeGuid, Guid discountGuid, DiscountDto discountDto);
        Task<bool> DeleteDiscountAsync(Guid storeGuid, Guid discountGuid);
        Task<DiscountDto> CreateDiscountAsync(Guid storeGuid, DiscountDto discountDto);
        Task<DiscountDto> GetDiscountAsync(Guid storeGuid);

        Task<PolicyDto> GetDiscountPolicyAsync(Guid storeGuid, Guid discountGuid);
        Task<PolicyDto> CreateDiscountPolicyAsync(Guid storeGuid, Guid discountGuid, PolicyDto policyDto);
        Task<bool> DeleteDiscountPolicyAsync(Guid storeGuid, Guid discountGuid, Guid policyGuid);
        Task<PolicyDto> CreateDiscountSubPolicy(Guid storeGuid, Guid discountGuid, Guid policyGuid, PolicyDto policyDto);

        Task ApproveOffer(Guid ownerGuid, Guid productOfferGuid);

        Task DeclineOffer(Guid ownerGuid, Guid productOfferGuid);

        Task<ProductOfferDto> MakeCounterOffer(Guid ownerGuid, decimal counterOfferPrice, Guid offerGuid);

        Task<ProductOfferDto> GetProductOffer(Guid offerGuid);

        Task<IEnumerable<ProductOfferDto>> GetAllUserProductOffers(Guid userGuid);

        Task<IEnumerable<ProductOfferDto>> GetAllOwnerProductOffers(Guid ownerGuid);
    }

}
