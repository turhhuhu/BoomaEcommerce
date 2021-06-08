using BoomaEcommerce.Domain;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using AutoMapper;
using BoomaEcommerce.Core.Exceptions;
using BoomaEcommerce.Data;
using BoomaEcommerce.Domain.Policies;
using BoomaEcommerce.Services.DTO;
using BoomaEcommerce.Services.DTO.Policies;
using FluentValidation;

namespace BoomaEcommerce.Services.Stores
{
    public class StoresService : IStoresService
    {
        private readonly ILogger<StoresService> _logger;
        private readonly IMapper _mapper;
        private readonly IStoreUnitOfWork _storeUnitOfWork;
        private readonly INotificationPublisher _notificationPublisher;

        public StoresService(ILogger<StoresService> logger,
            IMapper mapper,
            IStoreUnitOfWork storeUnitOfWork,
            INotificationPublisher notificationPublisher)
        {
            _logger = logger;
            _mapper = mapper;
            _storeUnitOfWork = storeUnitOfWork;
            _notificationPublisher = notificationPublisher;
        }

        public async Task<StoreDto> CreateStoreAsync(StoreDto store)
        {
            var newStore = _mapper.Map<Store>(store);
            try
            {
                _storeUnitOfWork.AttachNoChange(newStore.StoreFounder);

                await _storeUnitOfWork.StoreRepo.InsertOneAsync(newStore);


                var storeOwnerShip = new StoreOwnership
                {
                    Store = newStore,
                    User = newStore.StoreFounder
                };
                await _storeUnitOfWork.StoreOwnershipRepo.InsertOneAsync(storeOwnerShip);
                await _storeUnitOfWork.SaveAsync();
                return _mapper.Map<StoreDto>(newStore);
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
                return null;
            }
        }

        public async Task<IReadOnlyCollection<ProductDto>> GetProductsFromStoreAsync(Guid storeGuid)
        {
            try
            {
                _logger.LogInformation($"Getting products from UserStore with guid {storeGuid}");
                var products = await _storeUnitOfWork.ProductRepo.FilterByAsync(p => p.Store.Guid == storeGuid && !p.IsSoftDeleted);
                return _mapper.Map<IReadOnlyCollection<ProductDto>>(products.ToList());
            }
            catch (Exception e)
            {
                _logger.LogError($"Failed to get products from UserStore {storeGuid}", e);
                return null;
            }
        }

        public async Task<ProductDto> CreateStoreProductAsync(ProductDto productDto)
        {

            try
            {
                var product = _mapper.Map<Product>(productDto);
                var store = await _storeUnitOfWork.StoreRepo.FindByIdAsync(product.Store.Guid);
                if (store is null)
                {
                    _logger.LogWarning("create UserStore product failed because" +
                                       " UserStore with guid {UserDto} does not exists", product.Store.Guid);
                    return null;
                }
                await _storeUnitOfWork.ProductRepo.InsertOneAsync(product);
                return _mapper.Map<ProductDto>(product);
            }
            catch (Exception e)
            {
                _logger.LogError(e,"Failed to create product with UserDto {ProductGuid}," +
                                   " for UserStore with guid {StoreGuid}", productDto.Guid, productDto.StoreGuid);
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

            Product product = null;
            try
            {
                _logger.LogInformation($"Updating product with guid {productDto.Guid}");
                product = await _storeUnitOfWork.ProductRepo.FindByIdAsync(productDto.Guid);
                await product.ProductLock.WaitAsync();
                if (product.IsSoftDeleted) return false;
                var storeProduct = await _storeUnitOfWork.StoreRepo.FindByIdAsync(product.Store.Guid);
                if (storeProduct is null)
                {
                    _logger.LogWarning("update UserStore product failed because" +
                                       " UserStore with guid {UserDto} does not exists", product.Store.Guid);
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
            finally
            {
                product?.ProductLock.Release();
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
                _logger.LogInformation($"Deleting UserStore with guid: {storeGuid}");
                await _storeUnitOfWork.StoreRepo.DeleteByIdAsync(storeGuid);
                await _storeUnitOfWork.SaveAsync();
                return true;
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Failed to delete UserStore with guid {storeGuid}");
                return false;
            }
        }

        public async Task<IReadOnlyCollection<StorePurchaseDto>> GetStorePurchaseHistoryAsync(Guid storeGuid)
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



        public async Task<bool> RemoveStoreOwnerAsync(Guid ownerGuidRemoveFrom ,Guid ownerGuid)
        {
            try
            {
                var storeOwnershipRemoveFrom = await _storeUnitOfWork.StoreOwnershipRepo.FindByIdAsync(ownerGuidRemoveFrom);

                var owner = storeOwnershipRemoveFrom.GetOwner(ownerGuid);
                if (owner == null)
                {
                    return false;
                }

                var (owners, managers) = owner.GetSubordinates();

                await _storeUnitOfWork.StoreOwnershipRepo.DeleteByIdAsync(ownerGuid); // This will be implemented as on delete cascade
                storeOwnershipRemoveFrom.RemoveOwner(ownerGuid);

                owners.Add(owner);
                // await NotifyDismissal(storeOwnershipRemoveFrom, owners);
                _storeUnitOfWork.StoreOwnershipRepo.DeleteRange(owners);
                _storeUnitOfWork.StoreManagementRepo.DeleteRange(managers);
                await _storeUnitOfWork.SaveAsync();
                return true;
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Failed to delete StoreOwnerShip with guid {ownerGuid}");
                return false;
            }
        }

        private Task NotifyDismissal(StoreOwnership dismissingOwner, List<StoreOwnership> owners)
        {
            var notification = new RoleDismissalNotification(dismissingOwner.User, dismissingOwner.Store);
            owners.ForEach(owner => owner.User.AddNotification(notification));
            return _notificationPublisher.NotifyManyAsync(_mapper.Map<RoleDismissalNotificationDto>(notification), owners.Select(o => o.User.Guid));
        }


        public async Task<bool> NominateNewStoreOwnerAsync(Guid ownerGuid, StoreOwnershipDto newOwnerDto)
        {
            try
            {
                if (!await IsNotAlreadyNominated(newOwnerDto.Store.Guid, newOwnerDto.User.Guid))
                {
                    return false;
                }


                var ownerStoreOwnership = await _storeUnitOfWork.StoreOwnershipRepo.FindByIdAsync(ownerGuid);

                if (ownerStoreOwnership == null)
                {
                    return false;
                }

                var newOwner = _mapper.Map<StoreOwnership>(newOwnerDto);
                newOwner.User = await _storeUnitOfWork.UserRepo.FindByIdAsync(newOwnerDto.User.Guid);
                newOwner.Store = await _storeUnitOfWork.StoreRepo.FindByIdAsync(newOwnerDto.Store.Guid);
                ownerStoreOwnership.AddOwner(newOwner);
                _storeUnitOfWork.AttachNoChange(newOwner.User);

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

        public async Task<bool> NominateNewStoreManagerAsync(Guid ownerGuid, StoreManagementDto newManagementDto)
        {
            try
            {

                if (!await IsNotAlreadyNominated(newManagementDto.Store.Guid, newManagementDto.User.Guid))
                {
                    return false;
                }

                var ownerStoreOwnership = await _storeUnitOfWork.StoreOwnershipRepo.FindByIdAsync(ownerGuid);

                if (ownerStoreOwnership == null)
                {
                    return false;
                }

                var newManager = _mapper.Map<StoreManagement>(newManagementDto);
                newManager.Store = await _storeUnitOfWork.StoreRepo.FindByIdAsync(newManager.Store.Guid);
                ownerStoreOwnership.AddManager(newManager);
                newManager.User = await _storeUnitOfWork.UserRepo.FindByIdAsync(newManager.User.Guid);
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

        private async Task<bool> IsNotAlreadyNominated(Guid storeGuid, Guid userGuid)
        {
            try
            {
                //checking if the new ownerGuid is not already a UserStore ownerGuid or a UserStore ownerGuid
                var ownerShouldBeNull = await _storeUnitOfWork.StoreOwnershipRepo.FindOneAsync(storeOwnership =>
                    storeOwnership.User.Guid.Equals(userGuid) && storeOwnership.Store.Guid.Equals(storeGuid));

                var managerShouldBeNull = await _storeUnitOfWork.StoreManagementRepo.FindOneAsync(sm =>
                    sm.User.Guid.Equals(userGuid) && sm.Store.Guid.Equals(storeGuid));

                return ownerShouldBeNull == null && managerShouldBeNull == null;
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
                return false;
            }
        }

        public async Task UpdateManagerPermissionAsync(StoreManagementPermissionsDto smpDto)
        {
            try
            {
                var permission = _mapper.Map<StoreManagementPermissions>(smpDto);
                await _storeUnitOfWork.StoreManagementPermissionsRepo.ReplaceOneAsync(permission);
                await _storeUnitOfWork.SaveAsync();
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
            }
        }

        public async Task<StoreManagementDto> GetStoreManagementAsync(Guid storeManagementGuid)
        {
            try
            {
                var management = await _storeUnitOfWork.StoreManagementRepo.FindByIdAsync(storeManagementGuid);
                return _mapper.Map<StoreManagementDto>(management);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "failed to get UserStore management for guid {guid}", storeManagementGuid);
                return null;
            }
        }

        public async Task<StoreOwnershipDto> GetStoreOwnershipAsync(Guid storeOwnershipGuid)
        {
            try
            {
                var ownership = await _storeUnitOfWork.StoreOwnershipRepo.FindByIdAsync(storeOwnershipGuid);
                return _mapper.Map<StoreOwnershipDto>(ownership);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "failed to get UserStore ownership for guid {guid}", storeOwnershipGuid);
                return null;
            }
        }

        public async Task<StoreSellersDto> GetAllSellersInformationAsync(Guid storeGuid)
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
                       Store = storeManagement.Store,
                       Permissions = storeManagement.Permissions
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
                return new StoreSellersDto(storeOwnerDtos, storeManagementDtos);
                // Seller - A seller is either an Owner or a Manager.
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
                return null;
            }
        }

        public async Task<IReadOnlyCollection<StoreOwnershipDto>> GetAllStoreOwnerShipsAsync(Guid userGuid)
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
        
        public async Task<IReadOnlyCollection<StoreManagementDto>> GetAllStoreManagementsAsync(Guid userGuid)
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

        public async Task<StoreSellersDto> GetSubordinateSellersAsync(Guid storeOwnerGuid, int? level = null)
        {
            try
            {
                var storeOwner = await _storeUnitOfWork.StoreOwnershipRepo.FindByIdAsync(storeOwnerGuid);
                if (storeOwner == null)
                {
                    return null;
                }
                var (storeOwnerships, storeManagements) = storeOwner.GetSubordinates(level);

                return new StoreSellersDto(_mapper.Map<List<StoreOwnershipDto>>(storeOwnerships),
                    _mapper.Map<List<StoreManagementDto>>(storeManagements));
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Failed to get all subordinate sellers for UserStore ownerGuid with guid");
                return null;
            }
        }

        public async Task<StoreOwnershipDto> GetStoreOwnerShipAsync(Guid userGuid, Guid storeGuid)
        {
            try
            {
                var storeOwnership = await _storeUnitOfWork.StoreOwnershipRepo.FindOneAsync(ownership =>
                    ownership.User.Guid == userGuid && ownership.Store.Guid == storeGuid);
                return _mapper.Map<StoreOwnershipDto>(storeOwnership);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Failed to get all subordinate sellers for UserStore ownerGuid with guid");
                return null;
            }
        }

        public async Task<StoreManagementDto> GetStoreManagementAsync(Guid userGuid, Guid storeGuid)
        {
            try
            {
                var storeManagement = await _storeUnitOfWork.StoreManagementRepo.FindOneAsync(management =>
                    management.User.Guid == userGuid && management.Store.Guid == storeGuid);
                return _mapper.Map<StoreManagementDto>(storeManagement);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Failed to get all subordinate sellers for UserStore ownerGuid with guid");
                return null;
            }
        }

        public async Task<ProductDto> GetStoreProductAsync(Guid productGuid)
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

		public async Task<bool> RemoveManagerAsync(Guid ownershipToRemoveFrom, Guid managerToRemove)
        {
            try
            {
                var owner = await _storeUnitOfWork.StoreOwnershipRepo.FindByIdAsync(ownershipToRemoveFrom);

                _logger.LogInformation("Removing UserStore ownerGuid with guid {guid}", managerToRemove);

                if (!owner.RemoveManager(managerToRemove))
                {
                    return false;
                }

                await _storeUnitOfWork.StoreManagementRepo.DeleteByIdAsync(managerToRemove);
                await _storeUnitOfWork.StoreOwnershipRepo.ReplaceOneAsync(owner);
                await _storeUnitOfWork.SaveAsync();

                return true;
                
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Failed to remove ownerGuid with guid: {guid}", managerToRemove);
                return false;
            }
        }

        public async Task<PolicyDto> CreatePurchasePolicyAsync(Guid storeGuid, PolicyDto policyDto)
        {
            try
            {
                _logger.LogInformation("Making attempt to set store {storeGuid} with new policyDto.", storeGuid);
                var store = await _storeUnitOfWork.StoreRepo.FindByIdAsync(storeGuid);
                if (store == null)
                {
                    return null;
                }
                var policy = _mapper.Map<Policy>(policyDto);
                store.StorePolicy = policy;

                //TODO: remove when moving to EF core
                await _storeUnitOfWork.PolicyRepo.InsertOneAsync(policy);
                await _storeUnitOfWork.SaveAsync();
                _logger.LogInformation("Successfully set new policyDto for store with guid {storeGuid}", storeGuid);
                return _mapper.Map<PolicyDto>(policy);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Failed to set a new policyDto for store with guid {storeGuid}", storeGuid);
                return null;
            }
        }

        public async Task<PolicyDto> GetPolicyAsync(Guid storeGuid)
        {
            try
            {
                _logger.LogInformation("Making attempt to get policy from store with guid {storeGuid}", storeGuid);

                var policy =
                    (await _storeUnitOfWork.StoreRepo.FilterByAsync(
                        store => store.Guid == storeGuid,
                        store => store.StorePolicy))
                    .FirstOrDefault();

                if (policy == null || policy is EmptyPolicy)
                {
                    return null;
                }

                var policyWithChildren = await _storeUnitOfWork.PolicyRepo.FindByIdAsync(policy.Guid);

                _logger.LogInformation("Successfully got policy {policyGuid} from store with guid {storeGuid}.", policy.Guid, storeGuid);
                return _mapper.Map<PolicyDto>(policyWithChildren);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Failed to get store policy from store with guid {storeGuid}", storeGuid);
                return null;
            }
        }

        public async Task<bool> DeletePolicyAsync(Guid storeGuid, Guid policyGuid)
        {
            try
            {
                _logger.LogInformation("Making attempt to delete policy with guid {policyGuid} from store with guid {storeGuid}.", policyGuid, storeGuid);
                await _storeUnitOfWork.PolicyRepo.DeleteByIdAsync(policyGuid);
                await _storeUnitOfWork.SaveAsync();
                _logger.LogInformation("Successfully deleted policy with guid {policyGuid} from store with guid {storeGuid}.", policyGuid, storeGuid);
                return true;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Failed to delete policy with guid {policyGuid} from store with guid {storeGuid}", policyGuid, storeGuid);
                return false;
            }
        }

        public async Task<PolicyDto> AddPolicyAsync(Guid storeGuid, Guid policyGuid, PolicyDto childPolicyDto)
        {
            try
            {
                _logger.LogInformation("Making attempt add new child policy to policy with guid {policyGuid}.",
                    policyGuid);
                var multiPolicy = await _storeUnitOfWork.PolicyRepo.FindByIdAsync<MultiPolicy>(policyGuid);
                if (multiPolicy == null)
                {
                    return null;
                }

                var childPolicy = _mapper.Map<Policy>(childPolicyDto);
                multiPolicy.AddPolicy(childPolicy);

                //TODO: remove when moving to EF core
                await _storeUnitOfWork.PolicyRepo.InsertOneAsync(childPolicy);

                await _storeUnitOfWork.SaveAsync();
                _logger.LogInformation("Successfully added new child policy for policy with guid {policyGuid}",
                    policyGuid);
                return _mapper.Map<PolicyDto>(childPolicy);
            }
            catch (PolicyValidationException pe)
            {
                _logger.LogError(pe, "Failed to add policy.");
                throw;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Failed to add a new child policy for policy with guid {policyGuid}", policyGuid);
                return null;
            }
        }
    }
}
