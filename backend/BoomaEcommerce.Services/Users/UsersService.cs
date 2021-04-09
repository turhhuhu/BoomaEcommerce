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
using BoomaEcommerce.Services.Stores;

namespace BoomaEcommerce.Services.Users
{
    public class UsersService : IUsersService
    {        
        private readonly IMapper _mapper;
        private readonly ILogger<PurchasesService> _logger;
        private readonly IRepository<StoreOwnership> _storeOwnershipRepository;       
        private readonly IRepository<StoreManagement> _storeManagementRepository;
        private readonly IRepository<StoreManagementPermission> _permissionsRepository;


        public UsersService(IMapper mapper, ILogger<PurchasesService> logger, 
             IRepository<StoreOwnership> storeOwnershipRepository,
             IRepository<StoreManagement> storeManagementRepository,
             IRepository<StoreManagementPermission> permissionRepository)
        {
            _mapper = mapper;
            _logger = logger;
            _storeOwnershipRepository = storeOwnershipRepository;
            _storeManagementRepository = storeManagementRepository;
            _permissionsRepository = permissionRepository;
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
                var managersTask = _storeManagementRepository.FilterByAsync(storeManagement =>
                   storeManagement.Store.Guid == storeGuid, storeManagement => 
                    new StoreManagement
                    {
                        Guid = storeManagement.Guid,
                        User = storeManagement.User
                    });

                var ownersTask =  _storeOwnershipRepository.FilterByAsync(storeOwnership =>
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
                return new StoreSellersResponse(storeOwnerDtos, storeManagementDtos);
                // Seller - A seller is either an Owner or a Manager.
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
                return null;
            }
        }

        public async Task<StoreSellersResponse> GetAllSubordinateSellers(Guid storeOwnerGuid)
        {
            try
            {
                var storeOwner = await _storeOwnershipRepository.FindByIdAsync(storeOwnerGuid);
                var (storeOwnerships, storeManagements) = storeOwner.GetSubordinates();

                return new StoreSellersResponse(_mapper.Map<List<StoreOwnershipDto>>(storeOwnerships),
                    _mapper.Map<List<StoreManagementDto>>(storeManagements));
            }
            catch (Exception e)
            {
                _logger.LogError("Failed to get all subordinate sellers for store owner with guid", e);
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
