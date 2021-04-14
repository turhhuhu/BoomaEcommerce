using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using BoomaEcommerce.Core;
using BoomaEcommerce.Services.DTO;
using BoomaEcommerce.Services.Exceptions;
using BoomaEcommerce.Services.Products;
using Microsoft.AspNetCore.Authorization;

namespace BoomaEcommerce.Services.Stores
{
    class SecuredStoreService : SecuredServiceBase, IStoresService
    {
        private readonly IStoresService _storeService;
        public SecuredStoreService(ClaimsPrincipal claims, IStoresService storeService) : base(claims)
        {
            _storeService = storeService;
        }

        public static bool CreateSecuredStoreService(string token, IStoresService next, out IStoresService storesService)
        {
            try
            {
                var claimsPrincipal = ParseClaimsPrincipal(token);
                storesService = new SecuredStoreService(claimsPrincipal, next);
                return true;
            }
            catch (Exception e)
            {
                storesService = null;
                return false;
            }
        }

        public async Task<IReadOnlyCollection<StorePurchaseDto>> GetStorePurchaseHistory(Guid storeGuid)
        {
            var method = typeof(SecuredProductService).GetMethod(nameof(DeleteProductAsync));
            CheckAuthorized(method);

            var userGuid = Claims.GetUserGuid();
            var storeOwner = await _storeService.GetStoreOwnerShip(storeGuid, userGuid);
            if (storeOwner != null)
            {
                return await _storeService.GetStorePurchaseHistory(storeGuid);
            }
            throw new UnAuthorizedException(nameof(GetStorePurchaseHistory), userGuid);
        }

        public Task CreateStoreAsync(StoreDto store)
        {
            var method = typeof(SecuredProductService).GetMethod(nameof(CreateStoreProductAsync));
            CheckAuthorized(method);

            return _storeService.CreateStoreAsync(store);
        }

        public async Task<bool> CreateStoreProductAsync(ProductDto product)
        {
            var method = typeof(SecuredProductService).GetMethod(nameof(CreateStoreProductAsync));
            CheckAuthorized(method);
            var userGuid = Claims.GetUserGuid();

            if (await CanPerformSellerAction(management => management.CanAddProduct, product.Store.Guid))
            {
                return await _storeService.CreateStoreProductAsync(product);
            }

            throw new UnAuthorizedException(nameof(CreateStoreProductAsync), userGuid);
        }

        public async Task<bool> DeleteProductAsync(Guid productGuid)
        {
            var method = typeof(SecuredProductService).GetMethod(nameof(DeleteProductAsync));
            CheckAuthorized(method);
            var userGuid = Claims.GetUserGuid();

            var product = await _storeService.GetStoreProduct(productGuid);

            if (await CanPerformSellerAction(management => management.CanDeleteProduct, product.Store.Guid))
            {
                return await _storeService.DeleteProductAsync(productGuid);
            }

            throw new UnAuthorizedException(nameof(DeleteProductAsync), userGuid);
        }

        public async Task<bool> UpdateProductAsync(ProductDto product)
        {
            var method = typeof(SecuredProductService).GetMethod(nameof(UpdateProductAsync));
            CheckAuthorized(method);
            var userGuid = Claims.GetUserGuid();

            if (await CanPerformSellerAction(management => management.CanUpdateProduct, product.Store.Guid))
            {
                return await _storeService.UpdateProductAsync(product);
            }

            throw new UnAuthorizedException(nameof(UpdateProductAsync), userGuid);
        }

        private async Task<bool> CanPerformSellerAction(Func<StoreManagementPermissionDto, bool> actionPredicate, Guid storeGuid)
        {
            var userGuid = Claims.GetUserGuid();
            var ownership = await _storeService.GetStoreOwnerShip(userGuid, storeGuid);
            if (ownership == null)
            {
                return false;
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

        public async Task<bool> DeleteStoreAsync(Guid storeGuid)
        {
            var userGuid = Claims.GetUserGuid();

            var store = await _storeService.GetStoreAsync(storeGuid);
            if (store.StoreFounder.Guid == userGuid)
            {
                return await _storeService.DeleteProductAsync(storeGuid);
            }
            throw new UnAuthorizedException(nameof(DeleteStoreAsync), userGuid);
        }

        public Task<bool> NominateNewStoreOwner(Guid owner, StoreOwnershipDto newOwnerDto)
        {
            return _storeService.NominateNewStoreOwner(owner, newOwnerDto);
        }

        public Task<bool> NominateNewStoreManager(Guid owner, StoreManagementDto newManagementDto)
        {
            return _storeService.NominateNewStoreManager(owner, newManagementDto);
        }

        public Task<StoreSellersResponse> GetAllSubordinateSellers(Guid storeOwnerGuid)
        {
            var method = typeof(SecuredProductService).GetMethod(nameof(DeleteProductAsync));
            CheckAuthorized(method);

            return _storeService.GetAllSubordinateSellers(storeOwnerGuid);
        }

        public Task<StoreOwnershipDto> GetStoreOwnerShip(Guid userGuid, Guid storeGuid)
        {
            var method = typeof(SecuredProductService).GetMethod(nameof(DeleteProductAsync));
            CheckAuthorized(method);

            return _storeService.GetStoreOwnerShip(userGuid, storeGuid);
        }

        public Task<StoreManagementDto> GetStoreManagement(Guid userGuid, Guid storeGuid)
        {
            var method = typeof(SecuredProductService).GetMethod(nameof(DeleteProductAsync));
            CheckAuthorized(method);

            return _storeService.GetStoreManagement(userGuid, storeGuid);
        }

        public Task<ProductDto> GetStoreProduct(Guid productGuid)
        {
            return _storeService.GetStoreProduct(productGuid);
        }
    }
}
