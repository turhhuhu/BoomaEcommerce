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

        public async Task<List<User>> GetAllSellersInformation(Guid storeGuid)
        {
            try
            {
                var managers =  _smRepository.FilterByAsync(storeManagement =>
                    storeManagement.Store.Id == storeGuid, storeManagement => storeManagement.User);
                var owners =  _soRepository.FilterByAsync(storeOwnership =>
                    storeOwnership.Store.Id == storeGuid, storeOwnership => storeOwnership.User);

                return (await Task.WhenAll(managers, owners)).SelectMany(x => x).ToList();

                // Seller - A seller is either an Owner or a Manager.
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
