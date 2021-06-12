using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using BoomaEcommerce.Core;
using BoomaEcommerce.Domain;
using BoomaEcommerce.Services.DTO;
using BoomaEcommerce.Core.Exceptions;
using BoomaEcommerce.Services.DTO.Discounts;
using BoomaEcommerce.Services.DTO.Policies;
using BoomaEcommerce.Services.DTO.ProductOffer;
using BoomaEcommerce.Services.Products;
using Microsoft.AspNetCore.Authorization;

namespace BoomaEcommerce.Services.Stores
{
    public class SecuredStoreService : SecuredServiceBase, IStoresService
    {
        private readonly IStoresService _storeService;
        public SecuredStoreService(ClaimsPrincipal claimsPrincipal, IStoresService storeService) : base(claimsPrincipal)
        {
            _storeService = storeService;
        }

        public SecuredStoreService(IStoresService storesService)
        {
            _storeService = storesService;
        }

        public static bool CreateSecuredStoreService(string token, string secret, IStoresService next, out IStoresService storesService)
        {
            try
            {
                var claimsPrincipal = ValidateToken(token, secret);
                storesService = new SecuredStoreService(claimsPrincipal, next);
                return true;
            }
            catch
            {
                storesService = null;
                return false;
            }
        }

        public async Task<IReadOnlyCollection<StorePurchaseDto>> GetStorePurchaseHistoryAsync(Guid storeGuid)
        {
            // authentication
            CheckAuthenticated();

            // role authorization
            if (CheckRoleAuthorized(UserRoles.AdminRole))
            {
                return await _storeService.GetStorePurchaseHistoryAsync(storeGuid);
            }

            // specific authorization
            var userGuid = ClaimsPrincipal.GetUserGuid();
            var storeOwner = await _storeService.GetStoreOwnerShipAsync(userGuid, storeGuid);
            if (storeOwner != null)
            {
                return await _storeService.GetStorePurchaseHistoryAsync(storeGuid);
            }
            throw new UnAuthorizedException(nameof(GetStorePurchaseHistoryAsync), userGuid);
        }


        public Task<StoreDto> CreateStoreAsync(StoreDto store)
        {
            ServiceUtilities.ValidateDto<StoreDto, StoreServiceValidators.CreateStore>(store);
            CheckAuthenticated();
            var userGuid = ClaimsPrincipal.GetUserGuid();
            store.FounderUserGuid = userGuid;
            return _storeService.CreateStoreAsync(store);
        }

        public async Task<ProductDto> CreateStoreProductAsync(ProductDto product)
        {
            ServiceUtilities.ValidateDto<ProductDto, StoreServiceValidators.CreateStoreProduct>(product);
            CheckAuthenticated();
            var userGuid = ClaimsPrincipal.GetUserGuid();

            if (await CanPerformSellerAction(management => management.CanAddProduct, product.StoreGuid))
            {
                return await _storeService.CreateStoreProductAsync(product);
            }

            throw new UnAuthorizedException(nameof(CreateStoreProductAsync), userGuid);
        }

        public async Task<bool> DeleteProductAsync(Guid productGuid)
        {
            CheckAuthenticated();
            var userGuid = ClaimsPrincipal.GetUserGuid();

            var product = await _storeService.GetStoreProductAsync(productGuid);

            if (product == null)
            {
                return false;
            }

            if (await CanPerformSellerAction(management => management.CanDeleteProduct, product.StoreGuid))
            {
                return await _storeService.DeleteProductAsync(productGuid);
            }

            throw new UnAuthorizedException(nameof(DeleteProductAsync), userGuid);
        }

        public async Task<bool> UpdateProductAsync(ProductDto product)
        {
            ServiceUtilities.ValidateDto<ProductDto, StoreServiceValidators.UpdateStoreProduct>(product);

            CheckAuthenticated();
            var userGuid = ClaimsPrincipal.GetUserGuid();

            if (await CanPerformSellerAction(management => management.CanUpdateProduct, product.StoreGuid))
            {
                return await _storeService.UpdateProductAsync(product);
            }

            throw new UnAuthorizedException(nameof(UpdateProductAsync), userGuid);
        }

        private async Task<bool> CanPerformSellerAction(Func<StoreManagementPermissionsDto, bool> actionPredicate, Guid storeGuid)
        {
            var userGuid = ClaimsPrincipal.GetUserGuid();
            var ownership = await _storeService.GetStoreOwnerShipAsync(userGuid, storeGuid);
            if (ownership != null)
            {
                return true;
            }
            var management = await _storeService.GetStoreManagementAsync(userGuid, storeGuid);

            return management != null && actionPredicate(management.Permissions);
        }
        public Task<IReadOnlyCollection<StoreDto>> GetStoresAsync()
        {
            return _storeService.GetStoresAsync();
        }

        public Task<StoreDto> GetStoreAsync(Guid storeGuid)
        {
            return _storeService.GetStoreAsync(storeGuid);
        }

        public async Task<bool> DeleteStoreAsync(Guid storeGuid)
        {
            CheckAuthenticated();

            if (CheckRoleAuthorized(UserRoles.AdminRole))
            {
                return await _storeService.DeleteProductAsync(storeGuid);
            }

            var userGuid = ClaimsPrincipal.GetUserGuid();

            var store = await _storeService.GetStoreAsync(storeGuid);
            if (store.FounderUserGuid == userGuid)
            {
                return await _storeService.DeleteProductAsync(storeGuid);
            }
            throw new UnAuthorizedException(nameof(DeleteStoreAsync), userGuid);
        }

        public async Task<bool> NominateNewStoreOwnerAsync(Guid ownerGuid, StoreOwnershipDto newOwnerDto)
        {
            ServiceUtilities.ValidateDto<StoreOwnershipDto, StoreServiceValidators.NominateNewStoreOwner>(newOwnerDto);
            CheckAuthenticated();
            var userGuidInClaims = ClaimsPrincipal.GetUserGuid();
            var storeOwner = await _storeService.GetStoreOwnershipAsync(ownerGuid);
            if (storeOwner != null && storeOwner.User.Guid == userGuidInClaims)
            {
                return await _storeService.NominateNewStoreOwnerAsync(ownerGuid, newOwnerDto);
            }

            throw new UnAuthorizedException(nameof(NominateNewStoreOwnerAsync), ownerGuid);
        }

        public async Task<bool> NominateNewStoreManagerAsync(Guid ownerGuid, StoreManagementDto newManagementDto)
        {
            ServiceUtilities.ValidateDto<StoreManagementDto, StoreServiceValidators.NominateNewStoreManager>(newManagementDto);
            CheckAuthenticated();
            var userGuidInClaims = ClaimsPrincipal.GetUserGuid();
            
            var storeOwner = await _storeService.GetStoreOwnershipAsync(ownerGuid);
            if (storeOwner != null && storeOwner.User.Guid == userGuidInClaims)
            {
                return await _storeService.NominateNewStoreManagerAsync(ownerGuid, newManagementDto);
            }

            throw new UnAuthorizedException(nameof(NominateNewStoreManagerAsync), ownerGuid);
        }

        public Task<IReadOnlyCollection<ProductDto>> GetProductsFromStoreAsync(Guid storeGuid)
        {
            return _storeService.GetProductsFromStoreAsync(storeGuid);
        }

        public async Task UpdateManagerPermissionAsync(StoreManagementPermissionsDto smpDto)
        {
            ServiceUtilities.ValidateDto<StoreManagementPermissionsDto, StoreServiceValidators.UpdateManagerPermission>(smpDto);
            var storeManagement = await _storeService.GetStoreManagementAsync(smpDto.Guid);
            var userGuidInClaims = ClaimsPrincipal.GetUserGuid();
            var owner = await _storeService.GetStoreOwnerShipAsync(userGuidInClaims, storeManagement.Store.Guid);
            if (owner != null)
            {
                var (_, managers) = await _storeService.GetSubordinateSellersAsync(owner.Guid, 0);
                if (managers.Exists(manager => manager.Guid == smpDto.Guid))
                {
                    await _storeService.UpdateManagerPermissionAsync(smpDto);
                    return;
                }
            }

            throw new UnAuthorizedException(nameof(UpdateManagerPermissionAsync), userGuidInClaims);
        }

        public async Task<StoreManagementDto> GetStoreManagementAsync(Guid storeManagementGuid)
        {
            CheckAuthenticated();
            var userGuid = ClaimsPrincipal.GetUserGuid();
            var storeManagement = await _storeService.GetStoreManagementAsync(storeManagementGuid);

            if (storeManagement != null && (storeManagement.User.Guid == userGuid
                || await CanPerformSellerAction(permissions => permissions.CanGetSellersInfo, storeManagement.Store.Guid)))
            {
                return storeManagement;
            }

            throw new UnAuthorizedException(nameof(GetStoreManagementAsync), userGuid);
        }

        public async Task<StoreOwnershipDto> GetStoreOwnershipAsync(Guid storeOwnershipGuid)
        {
            CheckAuthenticated();
            var userGuid = ClaimsPrincipal.GetUserGuid();
            var storeOwnership = await _storeService.GetStoreOwnershipAsync(storeOwnershipGuid);

            if (storeOwnership != null && (storeOwnership.User.Guid == userGuid
                || await CanPerformSellerAction(permissions => permissions.CanGetSellersInfo, storeOwnership.Store.Guid)))
            {
                return storeOwnership;
            }

            throw new UnAuthorizedException(nameof(GetStoreOwnershipAsync), userGuid);
        }

        public async Task<StoreSellersDto> GetSubordinateSellersAsync(Guid storeOwnerGuid, int? level)
        {
            CheckAuthenticated();

            var storeOwnership =  await _storeService.GetStoreOwnershipAsync(storeOwnerGuid);
            var userInClaims = ClaimsPrincipal.GetUserGuid();

            if (storeOwnership != null && await CanPerformSellerAction(permissions => permissions.CanGetSellersInfo, storeOwnership.Store.Guid))
            {
                return await _storeService.GetSubordinateSellersAsync(storeOwnerGuid, level);
            }

            throw new UnAuthorizedException(nameof(GetSubordinateSellersAsync), userInClaims);
        }

        public async Task<StoreOwnershipDto> GetStoreOwnerShipAsync(Guid userGuid, Guid storeGuid)
        {
            CheckAuthenticated();
            var userGuidInClaims = ClaimsPrincipal.GetUserGuid();

            if (await CanPerformSellerAction(permissions => permissions.CanGetSellersInfo, storeGuid))
            {
                return await _storeService.GetStoreOwnerShipAsync(userGuid, storeGuid);
            }
            throw new UnAuthorizedException(nameof(GetStoreOwnerShipAsync), userGuidInClaims);
        }

        public async Task<StoreManagementDto> GetStoreManagementAsync(Guid userGuid, Guid storeGuid)
        {
            CheckAuthenticated();
            
            var userGuidInClaims = ClaimsPrincipal.GetUserGuid();
            if (userGuidInClaims == userGuid || await CanPerformSellerAction(permissions => permissions.CanGetSellersInfo, storeGuid))
            {
                return await _storeService.GetStoreManagementAsync(userGuid, storeGuid);
            }
            throw new UnAuthorizedException(nameof(GetStoreManagementAsync), userGuidInClaims);

        }

        public async Task<StoreSellersDto> GetAllSellersInformationAsync(Guid storeGuid)
        {
            CheckAuthenticated();
            var userGuidInClaims = ClaimsPrincipal.GetUserGuid();
            if (await CanPerformSellerAction(permissions => permissions.CanGetSellersInfo, storeGuid))
            {
                return await _storeService.GetAllSellersInformationAsync(storeGuid);
            }
            throw new UnAuthorizedException(nameof(GetAllSellersInformationAsync), userGuidInClaims);
        }

        public Task<IReadOnlyCollection<StoreOwnershipDto>> GetAllStoreOwnerShipsAsync(Guid userGuid)
        {
            CheckAuthenticated();
            var userGuidInClaims = ClaimsPrincipal.GetUserGuid();
            if (userGuidInClaims == userGuid)
            {
                return _storeService.GetAllStoreOwnerShipsAsync(userGuid);
            }
            throw new UnAuthorizedException(nameof(GetAllStoreOwnerShipsAsync), userGuidInClaims);
        }

        public Task<IReadOnlyCollection<StoreManagementDto>> GetAllStoreManagementsAsync(Guid userGuid)
        {
            CheckAuthenticated();
            var userGuidInClaims = ClaimsPrincipal.GetUserGuid();
            if (userGuidInClaims == userGuid)
            {
                return _storeService.GetAllStoreManagementsAsync(userGuid);
            }
            throw new UnAuthorizedException(nameof(GetAllStoreManagementsAsync), userGuidInClaims);
        }

        public Task<ProductDto> GetStoreProductAsync(Guid productGuid)
        {
            return _storeService.GetStoreProductAsync(productGuid);
        }
        public async Task<bool> RemoveManagerAsync(Guid ownershipToRemoveFrom, Guid managerToRemove)
        {
            CheckAuthenticated();
            var userGuidInClaims = ClaimsPrincipal.GetUserGuid();
            var storeOwnership = await _storeService.GetStoreOwnershipAsync(ownershipToRemoveFrom);
            if (storeOwnership != null && userGuidInClaims == storeOwnership?.User.Guid)
            {
                var (_, managers) = await _storeService.GetSubordinateSellersAsync(storeOwnership.Guid, 0);
                if (managers.Exists(manager => manager.Guid == managerToRemove))
                {
                    return await _storeService.RemoveManagerAsync(ownershipToRemoveFrom, managerToRemove);
                }
            }
            throw new UnAuthorizedException(nameof(RemoveManagerAsync), userGuidInClaims);
        }

        public async Task<bool> RemoveStoreOwnerAsync(Guid ownerGuidRemoveFrom, Guid ownerGuidToRemove)
        {
            CheckAuthenticated();
            var userGuidInClaims = ClaimsPrincipal.GetUserGuid();

            var storeOwnershipRemoveFrom = await _storeService.GetStoreOwnershipAsync(ownerGuidRemoveFrom);

            if (storeOwnershipRemoveFrom != null && userGuidInClaims == storeOwnershipRemoveFrom?.User.Guid)
            {
                var subordinates = await _storeService.GetSubordinateSellersAsync(ownerGuidRemoveFrom, 0);
                if (subordinates.StoreOwners.Exists(o => o.Guid == ownerGuidToRemove))
                {
                    return await _storeService.RemoveStoreOwnerAsync(ownerGuidRemoveFrom, ownerGuidToRemove);
                }
            }
            throw new UnAuthorizedException(nameof(RemoveStoreOwnerAsync), userGuidInClaims);
        }


        public async Task<PolicyDto> AddPolicyAsync(Guid storeGuid, Guid policyGuid, PolicyDto childPolicyDto)
        {
            ServiceUtilities.ValidateDto<CreatePolicyDto, StoreServiceValidators.CreatePolicyValidator>(new CreatePolicyDto { PolicyToCreate = childPolicyDto });
            CheckAuthenticated();
            if (await CanPerformSellerAction(permissions => permissions.CanCreatePolicy, storeGuid))
            {
                return await _storeService.AddPolicyAsync(storeGuid, policyGuid, childPolicyDto);
            }

            throw new UnAuthorizedException(nameof(AddPolicyAsync), ClaimsPrincipal.GetUserGuid());
        }

        public async Task<bool> DeletePolicyAsync(Guid storeGuid, Guid policyGuid)
        {
            CheckAuthenticated();
            if (await CanPerformSellerAction(permissions => permissions.CanDeletePolicy, storeGuid))
            {
                return await _storeService.DeletePolicyAsync(storeGuid, policyGuid);
            }

            throw new UnAuthorizedException(nameof(DeletePolicyAsync), ClaimsPrincipal.GetUserGuid());
        }

        public async Task<PolicyDto> CreatePurchasePolicyAsync(Guid storeGuid, PolicyDto policyDto)
        {
            ServiceUtilities.ValidateDto<CreatePolicyDto, StoreServiceValidators.CreatePolicyValidator>(new CreatePolicyDto { PolicyToCreate = policyDto });
            CheckAuthenticated();
            if (await CanPerformSellerAction(permissions => permissions.CanCreatePolicy, storeGuid))
            {
                return await _storeService.CreatePurchasePolicyAsync(storeGuid, policyDto);
            }

            throw new UnAuthorizedException(nameof(CreatePurchasePolicyAsync), ClaimsPrincipal.GetUserGuid());
        }

        public async Task<PolicyDto> GetPolicyAsync(Guid storeGuid)
        {
            CheckAuthenticated();
            if (await CanPerformSellerAction(permissions => permissions.CanGetPolicyInfo, storeGuid))
            {
                return await _storeService.GetPolicyAsync(storeGuid);
            }

            throw new UnAuthorizedException(nameof(GetPolicyAsync), ClaimsPrincipal.GetUserGuid());
        }

        public async Task<DiscountDto> AddDiscountAsync(Guid storeGuid, Guid discountGuid, DiscountDto discountDto)
        {
            ServiceUtilities.ValidateDto<CreateDiscountDto, StoreServiceValidators.AddDiscountValidator>(new CreateDiscountDto { DiscountToCreate = discountDto });
            CheckAuthenticated();
            if (await CanPerformSellerAction(permissions => permissions.CanCreateDiscounts, storeGuid))
            {
                return await _storeService.AddDiscountAsync(storeGuid, discountGuid, discountDto);
            }

            throw new UnAuthorizedException(nameof(AddDiscountAsync), ClaimsPrincipal.GetUserGuid());
        }

        public async Task<bool> DeleteDiscountAsync(Guid storeGuid, Guid discountGuid)
        {
            CheckAuthenticated();
            if (await CanPerformSellerAction(permissions => permissions.CanDeleteDiscount, storeGuid))
            {
                return await _storeService.DeleteDiscountAsync(storeGuid, discountGuid);
            }

            throw new UnAuthorizedException(nameof(DeleteDiscountAsync), ClaimsPrincipal.GetUserGuid());
        }

        public async Task<DiscountDto> CreateDiscountAsync(Guid storeGuid, DiscountDto discountDto)
        {
            ServiceUtilities.ValidateDto<CreateDiscountDto, StoreServiceValidators.CreateDiscountAsync>(new CreateDiscountDto { DiscountToCreate = discountDto });
            CheckAuthenticated();
            if (await CanPerformSellerAction(permissions => permissions.CanCreateDiscounts, storeGuid))
            {
                return await _storeService.CreateDiscountAsync(storeGuid, discountDto);
            }

            throw new UnAuthorizedException(nameof(CreateDiscountAsync), ClaimsPrincipal.GetUserGuid());
        }

        public async Task<DiscountDto> GetDiscountAsync(Guid storeGuid)
        {
            CheckAuthenticated();
            if (await CanPerformSellerAction(permissions => permissions.CanGetDiscountInfo, storeGuid))
            {
                return await _storeService.GetDiscountAsync(storeGuid);
            }

            throw new UnAuthorizedException(nameof(GetDiscountAsync), ClaimsPrincipal.GetUserGuid());
        }

        public async Task<PolicyDto> GetDiscountPolicyAsync(Guid storeGuid, Guid discountGuid)
        {
            CheckAuthenticated();
            if (await CanPerformSellerAction(permissions => permissions.CanGetDiscountInfo, storeGuid))
            {
                return await _storeService.GetDiscountPolicyAsync(storeGuid, discountGuid);
            }

            throw new UnAuthorizedException(nameof(GetDiscountPolicyAsync), ClaimsPrincipal.GetUserGuid());
        }

        public async Task<PolicyDto> CreateDiscountPolicyAsync(Guid storeGuid, Guid discountGuid, PolicyDto policyDto)
        {
            ServiceUtilities.ValidateDto<CreatePolicyDto, StoreServiceValidators.CreatePolicyValidator>(new CreatePolicyDto { PolicyToCreate = policyDto });
            CheckAuthenticated();
            if (await CanPerformSellerAction(permissions => permissions.CanCreateDiscounts, storeGuid))
            {
                return await _storeService.CreateDiscountPolicyAsync(storeGuid, discountGuid, policyDto);
            }

            throw new UnAuthorizedException(nameof(CreateDiscountPolicyAsync), ClaimsPrincipal.GetUserGuid());
        }

        public async Task<bool> DeleteDiscountPolicyAsync(Guid storeGuid, Guid discountGuid, Guid policyGuid)
        {
            CheckAuthenticated();
            if (await CanPerformSellerAction(permissions => permissions.CanDeleteDiscount, storeGuid))
            {
                return await _storeService.DeleteDiscountPolicyAsync(storeGuid, discountGuid, policyGuid);
            }

            throw new UnAuthorizedException(nameof(DeleteDiscountPolicyAsync), ClaimsPrincipal.GetUserGuid());
        }

        public async Task<PolicyDto> CreateDiscountSubPolicy(Guid storeGuid, Guid discountGuid, Guid policyGuid, PolicyDto policyDto)
        {
            ServiceUtilities.ValidateDto<CreatePolicyDto, StoreServiceValidators.CreatePolicyValidator>(new CreatePolicyDto { PolicyToCreate = policyDto });
            CheckAuthenticated();
            if (await CanPerformSellerAction(permissions => permissions.CanCreateDiscounts, storeGuid))
            {
                return await _storeService.CreateDiscountSubPolicy(storeGuid, discountGuid, policyGuid, policyDto);
            }

            throw new UnAuthorizedException(nameof(CreateDiscountSubPolicy), ClaimsPrincipal.GetUserGuid());
        }

        public async Task ApproveOffer(Guid ownerGuid, Guid productOfferGuid)
        {
            CheckAuthenticated();
            var userGuidInClaims = ClaimsPrincipal.GetUserGuid();
            var owner = await _storeService.GetStoreOwnershipAsync(ownerGuid);
            var offer = await _storeService.GetProductOffer(productOfferGuid);
            if (owner != null && owner.User.Guid == userGuidInClaims && owner.Store.Guid ==  offer.Product.Store.Guid)
            {
                await _storeService.ApproveOffer(ownerGuid, productOfferGuid);
            }

            throw new UnAuthorizedException(nameof(ApproveOffer), ClaimsPrincipal.GetUserGuid());
        }

        public async Task DeclineOffer(Guid ownerGuid, Guid productOfferGuid)
        {
            CheckAuthenticated();
            var userGuidInClaims = ClaimsPrincipal.GetUserGuid();
            var owner = await _storeService.GetStoreOwnershipAsync(ownerGuid);
            var offer = await _storeService.GetProductOffer(productOfferGuid);
            if (owner != null && owner.User.Guid == userGuidInClaims && owner.Store.Guid == offer.Product.Store.Guid)
            {
                await _storeService.DeclineOffer(ownerGuid, productOfferGuid);
            }

            throw new UnAuthorizedException(nameof(DeclineOffer), ClaimsPrincipal.GetUserGuid());
        }

        public async Task<ProductOfferDto> MakeCounterOffer(Guid ownerGuid, decimal counterOfferPrice, Guid offerGuid)
        {
            CheckAuthenticated();
            var userGuidInClaims = ClaimsPrincipal.GetUserGuid();
            var owner = await _storeService.GetStoreOwnershipAsync(ownerGuid);
            var offer = await _storeService.GetProductOffer(productOfferGuid);
            if (owner != null && owner.User.Guid == userGuidInClaims && owner.Store.Guid == offer.Product.Store.Guid)
            {
                await _storeService.MakeCounterOffer(ownerGuid, counterOfferPrice, offerGuid);
            }

            throw new UnAuthorizedException(nameof(MakeCounterOffer), ClaimsPrincipal.GetUserGuid());
        }

        public Task<ProductOfferDto> GetProductOffer(Guid storeGuid, Guid userGuid, Guid offerGuid)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<ProductOfferDto>> GetAllUserProductOffers(Guid userGuid)
        {
            CheckAuthenticated();
            var userGuidInClaims = ClaimsPrincipal.GetUserGuid();
            if (userGuidInClaims == userGuid)
            {
                return await _storeService.GetAllUserProductOffers(userGuid);
            }

            throw new UnAuthorizedException(nameof(GetAllUserProductOffers), ClaimsPrincipal.GetUserGuid());
        }

        public async Task<IEnumerable<ProductOfferDto>> GetAllOwnerProductOffers(Guid ownerGuid)
        {
            CheckAuthenticated();
            var owner = await _storeService.GetStoreOwnershipAsync(ownerGuid);
            if (owner != null)
            {
                return await _storeService.GetAllOwnerProductOffers(ownerGuid);
            }

            throw new UnAuthorizedException(nameof(GetAllOwnerProductOffers), ClaimsPrincipal.GetUserGuid());
        }
    }
}
