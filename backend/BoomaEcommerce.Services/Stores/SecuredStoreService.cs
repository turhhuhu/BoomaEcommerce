using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using BoomaEcommerce.Core;
using BoomaEcommerce.Domain;
using BoomaEcommerce.Services.DTO;
using BoomaEcommerce.Core.Exceptions;
using BoomaEcommerce.Services.DTO.Policies;
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
                await _storeService.UpdateManagerPermissionAsync(smpDto);
                return;
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
            if (userGuidInClaims == storeOwnership?.User.Guid)
            {
                return await _storeService.RemoveManagerAsync(ownershipToRemoveFrom, managerToRemove);
            }

            throw new UnAuthorizedException(nameof(RemoveManagerAsync), userGuidInClaims);
        }

        public async Task<bool> RemoveStoreOwnerAsync(Guid ownerGuidRemoveFrom, Guid ownerGuidToRemove)
        {
            CheckAuthenticated();
            var userGuidInClaims = ClaimsPrincipal.GetUserGuid();
            var storeOwnershipRemoveFrom = await _storeService.GetStoreOwnershipAsync(ownerGuidRemoveFrom);
            if (userGuidInClaims == storeOwnershipRemoveFrom?.User.Guid)
            {
                var subordinate = await _storeService.GetSubordinateSellersAsync(ownerGuidRemoveFrom);
                var storeOwnershipToRemove = await _storeService.GetStoreOwnershipAsync(ownerGuidToRemove);
                if (subordinate.StoreOwners.Contains(storeOwnershipToRemove))
                    return await _storeService.RemoveStoreOwnerAsync(ownerGuidRemoveFrom, ownerGuidToRemove);
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
    }
}
