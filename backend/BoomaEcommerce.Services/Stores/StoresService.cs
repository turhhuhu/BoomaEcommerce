using BoomaEcommerce.Domain;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using BoomaEcommerce.Data;
using BoomaEcommerce.Services.DTO;
using FluentValidation;

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
            ServiceUtilities.ValidateDto<StoreDto, StoreServiceValidators.CreateStoreAsync>(store);
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

        public async Task<ProductDto> CreateStoreProductAsync(ProductDto productDto)
        {
            ServiceUtilities.ValidateDto<ProductDto, StoreServiceValidators.CreateStoreProductValidator>(productDto);

            try
            {
                var product = _mapper.Map<Product>(productDto);
                var storeProduct = await _storeUnitOfWork.StoreRepo.FindByIdAsync(product.Store.Guid);
                if (storeProduct is null)
                {
                    _logger.LogWarning("create store product failed because" +
                                       " store with guid {Guid} does not exists", product.Store.Guid);
                    return null;
                }
                /*if (!product.ValidateStorePolicy() || !product.ValidateAmount())
                {
                    return null;
                }*/
                if (!product.ValidateAmount())
                {
                    return null;
                }
                await _storeUnitOfWork.ProductRepo.InsertOneAsync(product);
                return _mapper.Map<ProductDto>(product);
            }
            catch (Exception e)
            {
                _logger.LogError(e,"Failed to create product with Guid {ProductGuid}," +
                                   " for store with guid {StoreGuid}", productDto.Guid, productDto.Store.Guid);
                return null;
            }
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
            ServiceUtilities.ValidateDto<ProductDto, StoreServiceValidators.UpdateProductAsync>(productDto);
            try
            {
                _logger.LogInformation($"Updating product with guid {productDto.Guid}");
                var product = await _storeUnitOfWork.ProductRepo.FindByIdAsync(productDto.Guid);
                if (product.IsSoftDeleted) return false;
                var storeProduct = await _storeUnitOfWork.StoreRepo.FindByIdAsync(product.Store.Guid);
                if (storeProduct is null)
                {
                    _logger.LogWarning("update store product failed because" +
                                       " store with guid {Guid} does not exists", product.Store.Guid);
                    return false;
                }
                product.Name = productDto.Name ?? product.Name;
                product.Amount = productDto.Amount ?? product.Amount;
                product.Price = productDto.Price ?? product.Price;
                product.Category = productDto.Category ?? product.Category;
                product.Rating = productDto.Rating ?? product.Rating;

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
                await _storeUnitOfWork.StoreOwnershipRepo.ReplaceOneAsync(ownerStoreOwnership);
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
            ServiceUtilities.ValidateDto<StoreManagementDto, StoreServiceValidators.NominateNewStoreManager>(newManagementDto);
            try
            {

                var ownerStoreOwnership = await ValidateInformation(owner, newManagementDto.Store.Guid, newManagementDto.User.Guid);

                if (ownerStoreOwnership == null)
                    return false;

                var newManager = _mapper.Map<StoreManagement>(newManagementDto);
                ownerStoreOwnership.StoreManagements.TryAdd(newManagementDto.Guid, newManager);

                await _storeUnitOfWork.StoreManagementRepo.InsertOneAsync(newManager);
                await _storeUnitOfWork.StoreOwnershipRepo.ReplaceOneAsync(ownerStoreOwnership);
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

        public async Task<IReadOnlyCollection<StoreOwnershipDto>> GetAllStoreOwnerShips(Guid userGuid)
        {
            try
            {
                var storeOwnerships = (await _storeUnitOfWork.StoreOwnershipRepo
                    .FilterByAsync(storeOwnership => storeOwnership.User.Guid == userGuid)).ToList();
                return _mapper.Map<List<StoreOwnershipDto>>(storeOwnerships);
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
                return null;
            }
        }
        
        public async Task<IReadOnlyCollection<StoreManagementDto>> GetAllStoreManagements(Guid userGuid)
        {
            try
            {
                var storeManagements = (await _storeUnitOfWork.StoreManagementRepo
                    .FilterByAsync(storeManagement => storeManagement.User.Guid == userGuid)).ToList();
                return _mapper.Map<List<StoreManagementDto>>(storeManagements);
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

        public async Task<StoreOwnershipDto> GetStoreOwnerShip(Guid userGuid, Guid storeGuid)
        {
            try
            {
                var storeOwnership = await _storeUnitOfWork.StoreOwnershipRepo.FindOneAsync(ownership =>
                    ownership.User.Guid == userGuid && ownership.Store.Guid == storeGuid);
                return _mapper.Map<StoreOwnershipDto>(storeOwnership);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Failed to get all subordinate sellers for store owner with guid");
                return null;
            }
        }

        public async Task<StoreManagementDto> GetStoreManagement(Guid userGuid, Guid storeGuid)
        {
            try
            {
                var storeManagement = await _storeUnitOfWork.StoreManagementRepo.FindOneAsync(management =>
                    management.User.Guid == userGuid && management.Store.Guid == storeGuid);
                return _mapper.Map<StoreManagementDto>(storeManagement);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Failed to get all subordinate sellers for store owner with guid");
                return null;
            }
        }

        public async Task<ProductDto> GetStoreProduct(Guid productGuid)
        {
            try
            {
                _logger.LogInformation($"Getting product with guid {productGuid}");
                var product = await _storeUnitOfWork.ProductRepo.FindByIdAsync(productGuid);
                return product.IsSoftDeleted 
                    ? null 
                    : _mapper.Map<ProductDto>(product);
            }
            catch (Exception e)
            {
                _logger.LogError("Failed to get productDto", e);
                return null;
            }
        }


		public async Task<Boolean> RemoveManager(Guid removeOwner, Guid removeManager)
        {
            try
            {
               
                var canRemove = await ValidateOwnerRemovingManagerDetails(removeOwner, removeManager);
                if (canRemove)
                {
                    _logger.LogInformation($"Removing store manager with guid {removeManager}");
                    await _storeUnitOfWork.StoreManagementRepo.DeleteOneAsync(
                        manager => manager.User.Guid == removeManager);
                    var owner = await _storeUnitOfWork.StoreOwnershipRepo.FindByIdAsync(removeOwner);
                    StoreManagement removed;
                    owner.StoreManagements.Remove(removeManager,out removed);
                    await _storeUnitOfWork.StoreOwnershipRepo.ReplaceOneAsync(owner);
                    await _storeUnitOfWork.SaveAsync();

                    return true;
                }
                return false;
            }
            catch (Exception e)
            {
                _logger.LogError("Failed to remove manager", e);
                return false;
            }
        }

        public async Task<Boolean> ValidateOwnerRemovingManagerDetails(Guid removeOwner, Guid removeManager)
        {
            try
            {
                var owner = await _storeUnitOfWork.StoreOwnershipRepo.FindByIdAsync(removeOwner);
                var manager = await _storeUnitOfWork.StoreManagementRepo.FindByIdAsync(removeManager);
                var nominatedByOwner = owner.StoreManagements.TryGetValue(removeManager,out manager);
                if (!nominatedByOwner)
                    return false;
                return true;
                
            }
            catch (Exception e)
            {
                _logger.LogError("Failed to remove manager", e);
                return false;
            }
        }
    }
}
