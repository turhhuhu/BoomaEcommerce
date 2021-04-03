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
        private readonly IRepository<StoreOwnership> _soRepository;

        public UsersService(ILogger<StoreManagement> logger,
            IMapper mapper,
            IRepository<StoreManagement> smRepository,
            IRepository<StoreOwnership> soRepository)
        {
            _logger = logger;
            _mapper = mapper;
            _smRepository = smRepository;
            _soRepository = soRepository;
        }

        public async Task<List<object>> GetAllSellersInformation(Guid storeGuid)
        {
            try
            {
                var managers = await _smRepository.FilterByAsync(storeManagement =>
                    storeManagement.Store.Id == storeGuid);
                var owners = await _soRepository.FilterByAsync(storeOwnership =>
                    storeOwnership.Store.Id == storeGuid);

                // Seller - A seller is either an Owner or a Manager.

                var sellers = new List<object>();
                sellers.AddRange(managers.ToList());
                sellers.AddRange(owners.ToList());

                return sellers;
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
                return null;
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
