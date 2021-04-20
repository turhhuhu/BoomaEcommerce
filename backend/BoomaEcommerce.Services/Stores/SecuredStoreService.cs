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
        public async Task<IReadOnlyCollection<StorePurchaseDto>> GetStorePurchaseHistory(Guid storeGuid)
        {
            // authentication
            CheckAuthenticated();

            // role authorization
            var method = typeof(SecuredStoreService).GetMethod(nameof(GetStorePurchaseHistory));
            if (CheckRoleAuthorized(method))
            {
                return await _storeService.GetStorePurchaseHistory(storeGuid);
            }

            // specific authorization
            var userGuid = ClaimsPrincipal.GetUserGuid();
            var storeOwner = await _storeService.GetStoreOwnerShip(storeGuid, userGuid);
            if (storeOwner != null)
            {
                return await _storeService.GetStorePurchaseHistory(storeGuid);
            }
            throw new UnAuthorizedException(nameof(GetStorePurchaseHistory), userGuid);
        }

        public Task<StoreDto> CreateStoreAsync(StoreDto store)
        {
            CheckAuthenticated();
            return _storeService.CreateStoreAsync(store);
        }

        public async Task<ProductDto> CreateStoreProductAsync(ProductDto product)
        {
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

            var product = await _storeService.GetStoreProduct(productGuid);

            if (await CanPerformSellerAction(management => management.CanDeleteProduct, product.Store.Guid))
            {
                return await _storeService.DeleteProductAsync(productGuid);
            }

            throw new UnAuthorizedException(nameof(DeleteProductAsync), userGuid);
        }

        public async Task<bool> UpdateProductAsync(ProductDto product)
        {
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
            var ownership = await _storeService.GetStoreOwnerShip(userGuid, storeGuid);
            if (ownership != null)
            {
                return true;
            }
            var management = await _storeService.GetStoreManagement(userGuid, storeGuid);

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

        public Task<bool> NominateNewStoreOwner(Guid owner, StoreOwnershipDto newOwnerDto)
        {
            CheckAuthenticated();
            return _storeService.NominateNewStoreOwner(owner, newOwnerDto);
        }

        public Task<bool> NominateNewStoreManager(Guid owner, StoreManagementDto newManagementDto)
        {
            CheckAuthenticated();
            return _storeService.NominateNewStoreManager(owner, newManagementDto);
        }

        public Task<StoreSellersResponse> GetAllSubordinateSellers(Guid storeOwnerGuid)
        {
            CheckAuthenticated();
            return _storeService.GetAllSubordinateSellers(storeOwnerGuid);
        }

        public async Task<StoreOwnershipDto> GetStoreOwnerShip(Guid userGuid, Guid storeGuid)
        {
            CheckAuthenticated();
            var userGuidInClaims = ClaimsPrincipal.GetUserGuid();

            if (await CanPerformSellerAction(permissions => permissions.CanGetSellersInfo, storeGuid))
            {
                return await _storeService.GetStoreOwnerShip(userGuid, storeGuid);
            }
            throw new UnAuthorizedException(nameof(GetStoreOwnerShip), userGuidInClaims);
        }

        public async Task<StoreManagementDto> GetStoreManagement(Guid userGuid, Guid storeGuid)
        {
            CheckAuthenticated();
            var userGuidInClaims = ClaimsPrincipal.GetUserGuid();
            if (await CanPerformSellerAction(permissions => permissions.CanGetSellersInfo, storeGuid))
            {
                return await _storeService.GetStoreManagement(userGuid, storeGuid);
            }
            throw new UnAuthorizedException(nameof(GetStoreManagement), userGuidInClaims);

        }

        public async Task<StoreSellersResponse> GetAllSellersInformation(Guid storeGuid)
        {
            CheckAuthenticated();
            var userGuidInClaims = ClaimsPrincipal.GetUserGuid();
            if (await CanPerformSellerAction(permissions => permissions.CanGetSellersInfo, storeGuid))
            {
                return await _storeService.GetAllSellersInformation(storeGuid);
            }
            throw new UnAuthorizedException(nameof(GetAllSellersInformation), userGuidInClaims);
        }

        public Task<IReadOnlyCollection<StoreOwnershipDto>> GetAllStoreOwnerShips(Guid userGuid)
        {
            CheckAuthenticated();
            var userGuidInClaims = ClaimsPrincipal.GetUserGuid();
            if (userGuidInClaims == userGuid)
            {
                return _storeService.GetAllStoreOwnerShips(userGuid);
            }
            throw new UnAuthorizedException(nameof(GetAllStoreOwnerShips), userGuidInClaims);
        }

        public Task<IReadOnlyCollection<StoreManagementDto>> GetAllStoreManagements(Guid userGuid)
        {
            CheckAuthenticated();
            var userGuidInClaims = ClaimsPrincipal.GetUserGuid();
            if (userGuidInClaims == userGuid)
            {
                return _storeService.GetAllStoreManagements(userGuid);
            }
            throw new UnAuthorizedException(nameof(GetAllStoreManagements), userGuidInClaims);
        }

        public Task<ProductDto> GetStoreProduct(Guid productGuid)
        {
            return _storeService.GetStoreProduct(productGuid);
        }
    }
}
