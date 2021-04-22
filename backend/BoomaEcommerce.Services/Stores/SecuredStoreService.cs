using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using BoomaEcommerce.Core;
using BoomaEcommerce.Domain;
using BoomaEcommerce.Services.DTO;
using BoomaEcommerce.Core.Exceptions;
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

        [Authorize(Roles = UserRoles.AdminRole)]
        public async Task<IReadOnlyCollection<StorePurchaseDto>> GetStorePurchaseHistoryAsync(Guid storeGuid)
        {
            // authentication
            CheckAuthenticated();

            // role authorization
            var method = typeof(SecuredStoreService).GetMethod(nameof(GetStorePurchaseHistoryAsync));
            if (CheckRoleAuthorized(method))
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
            return _storeService.CreateStoreAsync(store);
        }

        public async Task<ProductDto> CreateStoreProductAsync(ProductDto product)
        {
            ServiceUtilities.ValidateDto<ProductDto, StoreServiceValidators.CreateStoreProduct>(product);
            CheckAuthenticated();
            var userGuid = ClaimsPrincipal.GetUserGuid();

            if (await CanPerformSellerAction(management => management.CanAddProduct, product.Store.Guid))
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

            if (await CanPerformSellerAction(management => management.CanDeleteProduct, product.Store.Guid))
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

            if (await CanPerformSellerAction(management => management.CanUpdateProduct, product.Store.Guid))
            {
                return await _storeService.UpdateProductAsync(product);
            }

            throw new UnAuthorizedException(nameof(UpdateProductAsync), userGuid);
        }

        private async Task<bool> CanPerformSellerAction(Func<StoreManagementPermissionDto, bool> actionPredicate, Guid storeGuid)
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

        [Authorize(Roles = UserRoles.AdminRole)]
        public async Task<bool> DeleteStoreAsync(Guid storeGuid)
        {
            CheckAuthenticated();

            var method = typeof(SecuredStoreService).GetMethod(nameof(DeleteProductAsync));
            if (CheckRoleAuthorized(method))
            {
                return await _storeService.DeleteProductAsync(storeGuid);
            }

            var userGuid = ClaimsPrincipal.GetUserGuid();

            var store = await _storeService.GetStoreAsync(storeGuid);
            if (store.StoreFounder.Guid == userGuid)
            {
                return await _storeService.DeleteProductAsync(storeGuid);
            }
            throw new UnAuthorizedException(nameof(DeleteStoreAsync), userGuid);
        }

        public async Task<bool> NominateNewStoreOwnerAsync(Guid owner, StoreOwnershipDto newOwnerDto)
        {
            ServiceUtilities.ValidateDto<StoreOwnershipDto, StoreServiceValidators.NominateNewStoreOwner>(newOwnerDto);
            CheckAuthenticated();

            var storeOwner = await _storeService.GetStoreOwnerShipAsync(owner, newOwnerDto.Store.Guid);
            if (storeOwner != null)
            {
                return await _storeService.NominateNewStoreOwnerAsync(owner, newOwnerDto);
            }

            throw new UnAuthorizedException(nameof(NominateNewStoreOwnerAsync), owner);
        }

        public async Task<bool> NominateNewStoreManagerAsync(Guid manager, StoreManagementDto newManagementDto)
        {
            ServiceUtilities.ValidateDto<StoreManagementDto, StoreServiceValidators.NominateNewStoreManager>(newManagementDto);
            CheckAuthenticated();
            var storeOwner = await _storeService.GetStoreOwnerShipAsync(manager, newManagementDto.Store.Guid);
            if (storeOwner != null)
            {
                return await _storeService.NominateNewStoreManagerAsync(manager, newManagementDto);
            }

            throw new UnAuthorizedException(nameof(NominateNewStoreManagerAsync), manager);
        }

        public Task<IReadOnlyCollection<ProductDto>> GetProductsFromStoreAsync(Guid storeGuid)
        {
            return _storeService.GetProductsFromStoreAsync(storeGuid);
        }

        public Task<StoreSellersResponse> GetAllSubordinateSellersAsync(Guid storeOwnerGuid)
        {
            CheckAuthenticated();
            return _storeService.GetAllSubordinateSellersAsync(storeOwnerGuid);
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
            if (await CanPerformSellerAction(permissions => permissions.CanGetSellersInfo, storeGuid))
            {
                return await _storeService.GetStoreManagementAsync(userGuid, storeGuid);
            }
            throw new UnAuthorizedException(nameof(GetStoreManagementAsync), userGuidInClaims);

        }

        public async Task<StoreSellersResponse> GetAllSellersInformationAsync(Guid storeGuid)
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
        public async Task<bool> RemoveManagerAsync(Guid removeOwnership, Guid removeManagement)
        {
            CheckAuthenticated();
            return await _storeService.RemoveManagerAsync(removeOwnership,removeManagement);
           
            
            
            
            
        }
    }
}
