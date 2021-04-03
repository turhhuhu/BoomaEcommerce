using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using BoomaEcommerce.Data;
using BoomaEcommerce.Domain;
using BoomaEcommerce.Services.DTO;
using BoomaEcommerce.Services.Stores;
using Microsoft.Extensions.Logging;

namespace BoomaEcommerce.Services.Users
{
    public class UsersService : IUsersService
    {
        private readonly ILogger<StoreManagement> _logger;
        private readonly IMapper _mapper;
        private readonly IRepository<StoreManagement> _smRepository;

        public UsersService(ILogger<StoreManagement> logger,
            IMapper mapper,
            IRepository<StoreManagement> smRepository)
        {
            _logger = logger;
            _mapper = mapper;
            _smRepository = smRepository;
        }

        public async Task<bool> AddManagerPermissions(Guid smGuid, Guid storeGuid,
            List<StoreManagementPermission> permissions)
        {
            try
            {
                var manager = await _smRepository.FindOneAsync(storeManagement =>
                    storeManagement.Store.Guid == storeGuid &&
                    storeManagement.User.Guid == smGuid);


                if (manager == null)
                {
                    return false;
                }

                var succAddPermissions = await manager.AddPermissions(permissions);

                if (!succAddPermissions) return false;

                await _smRepository.ReplaceOneAsync(manager);

                return true;
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
                return false;
            }
        }

        public async Task<bool> RemoveManagerPermissions(Guid smGuid, Guid storeGuid, List<StoreManagementPermission> permissions)
        {
            try
            {
                var manager = await _smRepository.FindOneAsync(storeManagement =>
                    storeManagement.Store.Guid == storeGuid &&
                    storeManagement.User.Guid == smGuid);


                if (manager == null)
                {
                    return false;
                }

                var suRemovePermissions = await manager.RemovePermissions(permissions);

                if (!suRemovePermissions) return false;

                await _smRepository.ReplaceOneAsync(manager);

                return true;
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
                return false;
            }
        }

        public Task CreateStoreAsync(string userId, StoreDto store)
        {
            throw new NotImplementedException();
        }

        public Task<ShoppingCartDto> GetShoppingCartAsync(string userId)
        {
            throw new NotImplementedException();
        }

        public Task CreateShoppingBasketAsync(string userId, ShoppingBasketDto shoppingBasket)
        {
            throw new NotImplementedException();
        }

        public Task AddProductToShoppingBasketAsync(string userId, Guid storeGuid, PurchaseProductDto purchaseProduct)
        {
            throw new NotImplementedException();
        }

        public Task DeleteProductFromShoppingBasketAsync(string userId, PurchaseProductDto purchaseProduct,
            ShoppingBasketDto shoppingBasket)
        {
            throw new NotImplementedException();
        }

        public Task DeleteShoppingBasketAsync(string userId, Guid storeGuid)
        {
            throw new NotImplementedException();
        }

        public Task<UserDto> GetUserInfoAsync(string userId)
        {
            throw new NotImplementedException();
        }

        public Task UpdateUserInfoAsync(UserDto user)
        {
            throw new NotImplementedException();
        }
    }
}
