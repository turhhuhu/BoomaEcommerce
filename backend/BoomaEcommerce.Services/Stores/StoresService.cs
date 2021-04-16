﻿using BoomaEcommerce.Domain;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using BoomaEcommerce.Data;
using BoomaEcommerce.Services.DTO;

namespace BoomaEcommerce.Services.Stores
{
    public class StoresService : IStoresService
    {
        private readonly ILogger<StoresService> _logger;
        private readonly IMapper _mapper;
        private readonly IStoreUnitOfWork _storeUnitOfWork;

        public StoresService(ILogger<StoresService> logger,
            IMapper mapper,
            IStoreUnitOfWork storeUnitOfWork)
        {
            _logger = logger;
            _mapper = mapper;
            _storeUnitOfWork = storeUnitOfWork;
        }

        public async Task CreateStoreAsync(StoreDto store)
        {
            var newStore = _mapper.Map<Store>(store);
            try
            {
                await _storeUnitOfWork.StoreRepo.InsertOneAsync(newStore);
                var user = _mapper.Map<User>(newStore.StoreFounder);
                var storeOwnerShip = new StoreOwnership
                {
                    Store = newStore,
                    User = user
                };
                await _storeUnitOfWork.StoreOwnershipRepo.InsertOneAsync(storeOwnerShip);
                await _storeUnitOfWork.SaveAsync();
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message); 
            }
        }

        public Task<bool> CreateStoreProductAsync(ProductDto productDto)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> DeleteProductAsync(Guid productGuid)
        {
            try
            {
                _logger.LogInformation($"Deleting product with guid {productGuid}");
                var product = await _storeUnitOfWork.ProductRepo.FindByIdAsync(productGuid);
                if (product == null) return false;
                if (product.IsSoftDeleted) return false;
                product.IsSoftDeleted = true;

                await _storeUnitOfWork.ProductRepo.ReplaceOneAsync(product);
                return true;
            }
            catch (Exception e)
            {
                _logger.LogError($"Failed to delete product with guid {productGuid}", e);
                return false;
            }
        }

        public async Task<bool> UpdateProductAsync(ProductDto productDto)
        {
            try
            {
                _logger.LogInformation($"Updating product with guid {productDto.Guid}");
                var product = await _storeUnitOfWork.ProductRepo.FindByIdAsync(productDto.Guid);
                if (product.IsSoftDeleted) return false;
                product.Name = productDto.Name ?? product.Name;
                product.Amount = productDto.Amount ?? product.Amount;
                product.Price = productDto.Price ?? product.Price;
                product.Category = productDto.Category ?? product.Category;

                await _storeUnitOfWork.ProductRepo.ReplaceOneAsync(product);
                return true;
            }
            catch (Exception e)
            {
                _logger.LogError($"Failed to update product with guid {productDto.Guid}", e);
                return false;
            }
        }
        public async Task<IReadOnlyCollection<StoreDto>> GetStoresAsync()
        {
            try
            {
                _logger.LogInformation("Getting all stores.");
                var stores = await _storeUnitOfWork.StoreRepo.FindAllAsync();
                return _mapper.Map<IReadOnlyCollection<StoreDto>>(stores.ToList());
            }
            catch (Exception e)
            {
                _logger.LogError("Failed to get stores", e.Message);
                return null;
            }
        }

        public async Task<StoreDto> GetStoreAsync(Guid storeGuid)
        {
            try
            {
                var store = await _storeUnitOfWork.StoreRepo.FindOneAsync(s => s.Guid == storeGuid);
                return _mapper.Map<StoreDto>(store);
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
                return null; 
            }
        }

        public async Task<bool> DeleteStoreAsync(Guid storeGuid)
        {
            try
            {
                _logger.LogInformation($"Deleting store with guid: {storeGuid}");
                await _storeUnitOfWork.StoreRepo.DeleteByIdAsync(storeGuid);
                await _storeUnitOfWork.SaveAsync();
                return true;
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Failed to delete store with guid {storeGuid}");
                return false;
            }
        }

        public async Task<IReadOnlyCollection<StorePurchaseDto>> GetStorePurchaseHistory(Guid storeGuid)
        {
            try
            {
                var purchaseHistory = await _storeUnitOfWork.StorePurchaseRepo.FilterByAsync(p => p.Store.Guid == storeGuid);
                return _mapper.Map<IReadOnlyCollection<StorePurchaseDto>>(purchaseHistory.ToList());
            }
            catch(Exception e)
            {
                _logger.LogError(e.Message);
                return null;
            }
        }
        public async Task<bool> NominateNewStoreOwner(Guid owner, StoreOwnershipDto newOwnerDto)
        {
            try
            {
                var ownerStoreOwnership = await ValidateInformation(owner, newOwnerDto.Store.Guid, newOwnerDto.User.Guid);

                if (ownerStoreOwnership == null)
                    return false;

                var newOwner = _mapper.Map<StoreOwnership>(newOwnerDto);
                ownerStoreOwnership.StoreOwnerships.TryAdd(newOwnerDto.Guid, newOwner);

                await _storeUnitOfWork.StoreOwnershipRepo.InsertOneAsync(newOwner);
                await _storeUnitOfWork.SaveAsync();
                return true;

            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
                return false;
            }
        }

        public async Task<bool> NominateNewStoreManager(Guid owner, StoreManagementDto newManagementDto)
        {
            try
            {

                var ownerStoreOwnership = await ValidateInformation(owner, newManagementDto.Store.Guid, newManagementDto.User.Guid);

                if (ownerStoreOwnership == null)
                    return false;

                var newManager = _mapper.Map<StoreManagement>(newManagementDto);
                ownerStoreOwnership.StoreManagements.TryAdd(newManagementDto.Guid, newManager);

                await _storeUnitOfWork.StoreManagementRepo.InsertOneAsync(newManager);
                await _storeUnitOfWork.SaveAsync();
                return true;

            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
                return false;
            }
        }

        private async Task<StoreOwnership> ValidateInformation(Guid ownerGuid, Guid storeGuid, Guid userGuid)
        {
            try
            {
                //Checking if owner is owner in the relevant store 
                var ownerStoreOwnership = await _storeUnitOfWork.StoreOwnershipRepo.FindOneAsync(storeOwnership =>
                    storeOwnership.User.Guid == ownerGuid && storeOwnership.Store.Guid == storeGuid);

                if (ownerStoreOwnership == null)
                {
                    return null;
                }

                //checking if the new owner is not already a store owner or a store manager
                var ownerShouldBeNull = await _storeUnitOfWork.StoreOwnershipRepo.FindOneAsync(storeOwnership =>
                    storeOwnership.User.Guid.Equals(userGuid) && storeOwnership.Store.Guid.Equals(storeGuid));
                var managerShouldBeNull = await _storeUnitOfWork.StoreManagementRepo.FindOneAsync(sm =>
                    sm.User.Guid.Equals(userGuid) && sm.Store.Guid.Equals(storeGuid));

                if (ownerShouldBeNull != null || managerShouldBeNull != null)
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
        public async Task<StoreManagementPermissionDto> GetPermissions(Guid smGuid)
        {
            try
            {
                var permission = await _storeUnitOfWork.StoreManagementPermissionsRepo.FindOneAsync(perm => perm.Guid == smGuid);
                return _mapper.Map<StoreManagementPermissionDto>(permission);
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
                return null;
            }
        }

        public async Task UpdatePermission(StoreManagementPermissionDto smpDto)
        {
            try
            {
                var permission = _mapper.Map<StoreManagementPermission>(smpDto);
                await _storeUnitOfWork.StoreManagementPermissionsRepo.ReplaceOneAsync(permission);
                await _storeUnitOfWork.SaveAsync();
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
            }
        }

        public async Task<StoreSellersResponse> GetAllSellersInformation(Guid storeGuid)
        {
            try
            {
                var managersTask = _storeUnitOfWork.StoreManagementRepo.FilterByAsync(storeManagement =>
                    storeManagement.Store.Guid == storeGuid);

                var ownersTask = _storeUnitOfWork.StoreOwnershipRepo.FilterByAsync(storeOwnership =>
                    storeOwnership.Store.Guid == storeGuid);

                var managers = (await managersTask).Select(storeManagement =>
                   new StoreManagement
                   {
                       Guid = storeManagement.Guid,
                       User = storeManagement.User,
                       Store = storeManagement.Store
                   }).ToList();

                var owners = (await ownersTask).Select(storeOwnership =>
                    new StoreOwnership
                    {
                        Guid = storeOwnership.Guid,
                        User = storeOwnership.User,
                        Store = storeOwnership.Store
                    }).ToList();


                var storeManagementDtos = _mapper.Map<List<StoreManagementDto>>(managers);
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
                var storeOwner = await _storeUnitOfWork.StoreOwnershipRepo.FindByIdAsync(storeOwnerGuid);
                var (storeOwnerships, storeManagements) = storeOwner.GetSubordinates();

                return new StoreSellersResponse(_mapper.Map<List<StoreOwnershipDto>>(storeOwnerships),
                    _mapper.Map<List<StoreManagementDto>>(storeManagements));
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Failed to get all subordinate sellers for store owner with guid");
                return null;
            }
        }

        public Task<StoreOwnershipDto> GetStoreOwnerShip(Guid userGuid, Guid storeGuid)
        {
            throw new NotImplementedException();
        }

        public Task<StoreManagementDto> GetStoreManagement(Guid userGuid, Guid storeGuid)
        {
            throw new NotImplementedException();
        }

        public Task<ProductDto> GetStoreProduct(Guid productGuid)
        {
            throw new NotImplementedException();
        }
    }
}
