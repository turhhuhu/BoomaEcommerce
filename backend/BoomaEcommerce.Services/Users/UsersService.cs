using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using BoomaEcommerce.Data;
using BoomaEcommerce.Domain;
using BoomaEcommerce.Services.DTO;
using BoomaEcommerce.Services.External;
using BoomaEcommerce.Services.Purchases;
using Microsoft.Extensions.Logging;


namespace BoomaEcommerce.Services.Users
{
    public class UsersService : IUsersService
    {
        
        private readonly IMapper _mapper;
        private readonly ILogger<UsersService> _logger;
        private readonly IPaymentClient _paymentClient;
        private readonly IRepository<User> _userRepository;
        private readonly IRepository<Product> _productRepository;
        private readonly IRepository<Purchase> _purchaseRepository;
        private readonly IRepository<StoreOwnership> _storeOwnershipRepository;       
        private readonly IRepository<StoreManagement> _storeManagementRepository;


        public UsersService(IMapper mapper, ILogger<UsersService> logger,
             IRepository<User> userRepository, IRepository<Product> productRepository,
            IRepository<Purchase> purchaseRepository , IRepository<StoreOwnership> storeOwnershipRepository,
             IRepository<StoreManagement> storeManagementRepository)
        {
            _mapper = mapper;
            _logger = logger;
            _userRepository = userRepository;
            _productRepository = productRepository;
            _purchaseRepository = purchaseRepository;
            _storeOwnershipRepository = storeOwnershipRepository;
            _storeManagementRepository = storeManagementRepository;
        }

        public UsersService(IRepository<StoreOwnership> storeOwnershipRepository,
            IRepository<StoreManagement> storeManagementRepository)
        {
            _storeOwnershipRepository = storeOwnershipRepository;
            _storeManagementRepository = storeManagementRepository;
        }

        public UsersService(IMapper mapper, ILogger<UsersService> logger, IRepository<StoreOwnership> storeOwnershipRepository, IRepository<StoreManagement> storeManagementRepository)
        {
            _mapper = mapper;
            _logger = logger;
            _storeOwnershipRepository = storeOwnershipRepository;
            _storeManagementRepository = storeManagementRepository;
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

        public async Task<bool> NominateNewStoreOwner(Guid owner, StoreOwnershipDto newOwnerDto)
        {
            try
            {
                var ownerStoreOwnership = await ValidateInforamation(owner, newOwnerDto.Store.Guid, newOwnerDto.User.Guid);

                if (ownerStoreOwnership == null)
                    return false;
                
                var newOwner = _mapper.Map<StoreOwnership>(newOwnerDto);
                ownerStoreOwnership.StoreOwnerships.TryAdd(newOwnerDto.Guid,newOwner);

                await _storeOwnershipRepository.InsertOneAsync(newOwner);
                return true;
                
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
                return false;
            }
        }
        
        public async Task<bool> NominateNewStoreManager(Guid owner, StoreManagementDto newOwnerDto)
        {
            try
            {
                
                var ownerStoreOwnership = await ValidateInforamation(owner, newOwnerDto.Store.Guid, newOwnerDto.User.Guid);

                if (ownerStoreOwnership == null)
                    return false;
                
                var newManager = _mapper.Map<StoreManagement>(newOwnerDto);
                ownerStoreOwnership.StoreManagements.TryAdd(newOwnerDto.Guid,newManager);

                await _storeManagementRepository.InsertOneAsync(newManager);
                return true;
                
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
                return false;
            }
        }
        
        
        
        private async Task<StoreOwnership> ValidateInforamation(Guid ownerGuid, Guid StoreGuid, Guid userGuid)
        {
            try
            {
                //Checking if owner is owner in the relevant store 
                var ownerStoreOwnership = await _storeOwnershipRepository.FindOneAsync(storeOwnership =>
                    storeOwnership.User.Guid.Equals(ownerGuid) && storeOwnership.Store.Guid.Equals(StoreGuid));
                
                //checking if the new owner is not already a store owner or a store manager
                var ownerShouldBeNull = await _storeOwnershipRepository.FindOneAsync(storeOwnership =>
                    storeOwnership.User.Guid.Equals(userGuid) && storeOwnership.Store.Guid.Equals(StoreGuid));
                var managerShouldBeNull = await _storeManagementRepository.FindOneAsync(sm =>
                    sm.User.Guid.Equals(userGuid) && sm.Store.Guid.Equals(StoreGuid));
                
                if (ownerShouldBeNull != null || managerShouldBeNull != null || ownerStoreOwnership == null)
                {
                    return null;
                }


                return ownerStoreOwnership;

            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
                return null;
            }
        }

        
        
    }
}
