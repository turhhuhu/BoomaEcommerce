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
        private IRepository<StoreManagementPermission> _permissionsRepository;
        private readonly IRepository<StoreOwnership> _soRepository;

        public UsersService(ILogger<StoreManagement> logger,
            IMapper mapper,
            IRepository<StoreManagement> smRepository,
            IRepository<StoreManagementPermission> permRepository)
            IRepository<StoreOwnership> soRepository)
        {
            _logger = logger;
            _mapper = mapper;
            _smRepository = smRepository;
            _permissionsRepository = permRepository;
            _soRepository = soRepository;
        }


        public async Task<StoreManagementPermissionDto> GetPermissions(Guid smGuid)
        {
            try
            {
                var permission = await _permissionsRepository.FindOneAsync(perm => perm.Guid == smGuid);
                return _mapper.Map<StoreManagementPermissionDto>(permission);
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
                return null;
            }
        }

        public Task UpdatePermission(StoreManagementPermissionDto smpDto)
        {
            try
            {
                var permission = _mapper.Map<StoreManagementPermission>(smpDto);
                return _permissionsRepository.ReplaceOneAsync(permission);
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
                return null;
            }
        }

        public async Task<StoreSellersResponse> GetAllSellersInformation(Guid storeGuid)
        {
            try
            {
                var managersTask = _smRepository.FilterByAsync(storeManagement =>
                   storeManagement.Store.Guid == storeGuid, storeManagement => 
                    new StoreManagement
                    {
                        Guid = storeManagement.Guid,
                        User = storeManagement.User
                    });

                var ownersTask =  _soRepository.FilterByAsync(storeOwnership =>
                    storeOwnership.Store.Guid == storeGuid, storeOwnership =>
                    new StoreOwnership
                    {
                        Guid = storeOwnership.Guid,
                        User = storeOwnership.User
                    });

                var managers = (await managersTask).ToList();
                var owners = (await ownersTask).ToList();


                var storeManagementDtos =  _mapper.Map<List<StoreManagementDto>>(managers);
                var storeOwnerDtos = _mapper.Map<List<StoreOwnershipDto>>(owners);
                return new StoreSellersResponse
                {
                    StoreManagers = storeManagementDtos,
                    StoreOwners = storeOwnerDtos
                };
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
