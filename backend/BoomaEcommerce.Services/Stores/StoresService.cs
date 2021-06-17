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
using BoomaEcommerce.Domain.Discounts;
using BoomaEcommerce.Domain.Policies;
using BoomaEcommerce.Services.DTO;
using BoomaEcommerce.Services.DTO.Discounts;
using BoomaEcommerce.Services.DTO.Policies;
using FluentValidation;
using BoomaEcommerce.Domain.ProductOffer;
using BoomaEcommerce.Services.DTO.ProductOffer;

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
                newStore.StoreFounder = await _storeUnitOfWork.UserRepo.FindByIdAsync(store.FounderUserGuid);
                if (newStore.StoreFounder == null)
                {
                    return null;
                }

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
                product.Store = await _storeUnitOfWork.StoreRepo.FindByIdAsync(product.Store.Guid);
                if (product.Store is null)
                {
                    _logger.LogWarning("create UserStore product failed because" +
                                       " UserStore with guid {UserDto} does not exists", productDto.StoreGuid);
                    return null;
                }
                await _storeUnitOfWork.ProductRepo.InsertOneAsync(product);
                await _storeUnitOfWork.SaveAsync();
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
                await _storeUnitOfWork.SaveAsync();
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

                await _storeUnitOfWork.SaveAsync();
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

                var storeGuid = storeOwnershipRemoveFrom.Store.Guid;

                var owner = storeOwnershipRemoveFrom.GetOwner(ownerGuid);
                if (owner == null)
                {
                    return false;
                }

                var (owners, managers) = owner.GetSubordinates();

                storeOwnershipRemoveFrom.RemoveOwner(ownerGuid);

                owners.Add(owner);
                await NotifyDismissal(storeOwnershipRemoveFrom, owners);
                _storeUnitOfWork.StoreOwnershipRepo.DeleteRange(owners);

                _storeUnitOfWork.StoreManagementRepo.DeleteRange(managers);

                await _storeUnitOfWork.SaveAsync();

                var isUpdated = await UpdateStoreOffers(storeGuid);

                if(isUpdated)
                    await _storeUnitOfWork.SaveAsync();
                return true;
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Failed to delete StoreOwnerShip with guid {ownerGuid}");
                return false;
            }
        }

        private async Task<bool> UpdateStoreOffers(Guid storeGuid)
        {
            var isUpdated = false;
            var storeOwners = (await _storeUnitOfWork.StoreOwnershipRepo.FilterByAsync(so => so.Store.Guid == storeGuid)).ToList();
            var offers = await _storeUnitOfWork.OffersRepo.FilterByAsync(o => o.Product.Store.Guid == storeGuid);

            foreach (var offer in offers)
            {
                var res = offer.CheckProductOfferState(storeOwners.ToList());
                if (res != ProductOfferState.Pending)
                    isUpdated = true;
            }

            return isUpdated;
        }

        private Task NotifyDismissal(StoreOwnership dismissingOwner, List<StoreOwnership> owners)
        {
            var notifications = new List<(Guid, NotificationDto)>();
            foreach (var owner in owners)
            {
                var notification = new RoleDismissalNotification(dismissingOwner.User, dismissingOwner.Store);
                _storeUnitOfWork.Attach(notification);
                owner.User.AddNotification(notification);
                notifications.Add((owner.User.Guid, _mapper.Map<NotificationDto>(notification)));
            }

            return _notificationPublisher.NotifyManyAsync(notifications);
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
                var permissions = _mapper.Map<StoreManagementPermissions>(smpDto);
                var management = await _storeUnitOfWork.StoreManagementRepo.FindByIdAsync(smpDto.Guid);
                management.Permissions = permissions;
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
                _logger.LogError(e, "failed to get UserStore management for guid {Guid}", storeManagementGuid);
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
                var managers = (await _storeUnitOfWork.StoreManagementRepo.FilterByAsync(storeManagement =>
                    storeManagement.Store.Guid == storeGuid)).ToList();

                var owners = (await _storeUnitOfWork.StoreOwnershipRepo
                    .FilterByAsync(storeOwnership => storeOwnership.Store.Guid == storeGuid)).ToList();


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

                var fullPolicy = await _storeUnitOfWork.PolicyRepo.FindByIdAsync(policy.Guid);

                _logger.LogInformation("Successfully got policy {policyGuid} from store with guid {storeGuid}.", policy.Guid, storeGuid);
                return _mapper.Map<PolicyDto>(fullPolicy);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Failed to get store policy from store with guid {storeGuid}", storeGuid);
                return null;
            }
        }

        public async Task<DiscountDto> AddDiscountAsync(Guid storeGuid, Guid discountGuid, DiscountDto discountDto)
        {
            try
            {
                _logger.LogInformation("Making attempt add new child discount to discount with guid {discountGuid}.", discountGuid);
                var compositeDiscount = await _storeUnitOfWork.DiscountRepo.FindByIdAsync<CompositeDiscount>(discountGuid);
                if (compositeDiscount == null)
                {
                    return null;
                }
                var childDiscount = _mapper.Map<Discount>(discountDto);
                compositeDiscount.AddToDiscountList(childDiscount);

                await _storeUnitOfWork.DiscountRepo.InsertOneAsync(childDiscount);

                await _storeUnitOfWork.SaveAsync();
                _logger.LogInformation("Successfully added new child discount for discount with guid {discountGuid}", discountGuid);
                return _mapper.Map<DiscountDto>(childDiscount);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Failed to add a new child discount for discount with guid {discountGuid}", discountGuid);
                return null;
            }
        }

        public async Task<bool> DeleteDiscountAsync(Guid storeGuid, Guid discountGuid)
        {
            try
            {
                _logger.LogInformation(
                    "Making attempt to delete discount with guid {discountGuid} from store with guid {storeGuid}.",
                    discountGuid, storeGuid);
                await _storeUnitOfWork.DiscountRepo.DeleteByIdAsync(discountGuid);
                await _storeUnitOfWork.SaveAsync();
                _logger.LogInformation(
                    "Successfully deleted discount with guid {discountGuid} from store with guid {storeGuid}.", discountGuid,
                    storeGuid);
                return true;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Failed to delete discount with guid {discountGuid} from store with guid {storeGuid}",
                    discountGuid, storeGuid);
                return false;
            }
        }

        public async Task<DiscountDto> CreateDiscountAsync(Guid storeGuid, DiscountDto discountDto)
        {
            try
            {
                _logger.LogInformation("Making attempt to set store {storeGuid} with new Discount.", storeGuid);
                var store = await _storeUnitOfWork.StoreRepo.FindByIdAsync(storeGuid);
                if (store == null)
                {
                    return null;
                }
                var discount = _mapper.Map<Discount>(discountDto);
                store.StoreDiscount = discount;

                await _storeUnitOfWork.DiscountRepo.InsertOneAsync(discount);

                await _storeUnitOfWork.SaveAsync();

                _logger.LogInformation("Successfully set new Discount for store with guid {storeGuid}", storeGuid);
                return _mapper.Map<DiscountDto>(discount);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Failed to set a new Discount for store with guid {storeGuid}", storeGuid);
                return null;
            }
        }

        public async Task<DiscountDto> GetDiscountAsync(Guid storeGuid)
        {
            try
            {
                _logger.LogInformation("Making attempt to get discount from store with guid {storeGuid}", storeGuid);

                var discount =
                    (await _storeUnitOfWork.StoreRepo.FilterByAsync(
                        store => store.Guid == storeGuid,
                        store => store.StoreDiscount))
                    .FirstOrDefault();

                if (discount == null || discount is EmptyDiscount)
                {
                    return null;
                }

                var fullDiscount = await _storeUnitOfWork.DiscountRepo.FindByIdAsync(discount.Guid);

                _logger.LogInformation("Successfully got discount {discountGuid} from store with guid {storeGuid}.", discount.Guid, storeGuid);
                return _mapper.Map<DiscountDto>(fullDiscount);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Failed to get store discount from store with guid {storeGuid}", storeGuid);
                return null;
            }
        }

        public async Task<PolicyDto> GetDiscountPolicyAsync(Guid storeGuid, Guid discountGuid)
        {
            try
            {
                _logger.LogInformation("Making attempt to get discount policy from store with guid {storeGuid}", storeGuid);

                var policy =
                    (await _storeUnitOfWork.DiscountRepo.FilterByAsync(
                        discount => discount.Guid == discountGuid,
                        discount => discount.Policy))
                    .FirstOrDefault();

                if (policy == null)
                {
                    return null;
                }

                var fullPolicy = await _storeUnitOfWork.PolicyRepo.FindByIdAsync(policy.Guid);

                _logger.LogInformation($"Successfully got policy {{policyGuid}} from {{discountGuid}} policy from store with guid {{storeGuid}}.", policy.Guid,discountGuid, storeGuid);
                return _mapper.Map<PolicyDto>(fullPolicy);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Failed to get store discount policy from store with guid {storeGuid}", storeGuid);
                return null;
            }
        }

        public async Task<PolicyDto> CreateDiscountPolicyAsync(Guid storeGuid, Guid discountGuid, PolicyDto policyDto)
        {
            try
            {
                _logger.LogInformation($"Making attempt to set store discount {discountGuid} with new Discount.", discountGuid);
                var storeDiscount = await _storeUnitOfWork.DiscountRepo.FindByIdAsync(discountGuid);
                if (storeDiscount == null)
                {
                    return null;
                }

                var policyToInsert = _mapper.Map<Policy>(policyDto);
                storeDiscount.Policy = policyToInsert;

                await _storeUnitOfWork.PolicyRepo.InsertOneAsync(policyToInsert);

                await _storeUnitOfWork.SaveAsync();

                _logger.LogInformation("Successfully set new policy for Discount {discountGuid} for store with guid {storeGuid}", discountGuid, storeGuid);
                return _mapper.Map<PolicyDto>(policyToInsert);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Failed to set a new Discount for store with guid {storeGuid}", storeGuid);
                return null;
            }
        }

        public async Task<bool> DeleteDiscountPolicyAsync(Guid storeGuid, Guid discountGuid, Guid policyGuid)
        {
            try
            {
                _logger.LogInformation("Making attempt to delete store discount policy with guid {policyGuid} from store with guid {storeGuid}.", policyGuid, storeGuid);
                await _storeUnitOfWork.PolicyRepo.DeleteByIdAsync(policyGuid);
                await _storeUnitOfWork.SaveAsync();
                _logger.LogInformation("Successfully deleted store discount policy with guid {policyGuid} from store with guid {storeGuid}.", policyGuid, storeGuid);
                return true;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Failed to delete store discount policy with guid {policyGuid} from store with guid {storeGuid}", policyGuid, storeGuid);
                return false;
            }
        }

        public async Task<PolicyDto> CreateDiscountSubPolicy(Guid storeGuid, Guid discountGuid, Guid policyGuid, PolicyDto policyDto)
        {
            try
            {
                _logger.LogInformation("Making attempt add new child policy to policy of discount with guid {discountGuid}.", discountGuid);
                var cPolicy = await _storeUnitOfWork.PolicyRepo.FindByIdAsync<MultiPolicy>(policyGuid);
                if (cPolicy == null)
                {
                    return null;
                }

                var childPolicy = _mapper.Map<Policy>(policyDto);
                cPolicy.AddPolicy(childPolicy);

                await _storeUnitOfWork.PolicyRepo.InsertOneAsync(childPolicy);

                await _storeUnitOfWork.SaveAsync();
                _logger.LogInformation("Successfully added new child policy to policy of discount with guid {discountGuid}", discountGuid);
                return _mapper.Map<PolicyDto>(childPolicy);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Failed to add a new child discount for discount with guid {discountGuid}", discountGuid);
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


        public async Task ApproveOffer(Guid ownerGuid, Guid productOfferGuid)
        {
            try
            {
                _logger.LogInformation($"Owner with guid {ownerGuid} is approving offer with guid {productOfferGuid}");

                var productOffer = await _storeUnitOfWork.OffersRepo.FindByIdAsync(productOfferGuid);
                var storeGuid = productOffer.Product.Store.Guid; 

                var owner = await _storeUnitOfWork.StoreOwnershipRepo.FindByIdAsync(ownerGuid);

                var ownersInStore =
                    await _storeUnitOfWork.StoreOwnershipRepo.FilterByAsync(o => o.Store.Guid == storeGuid);

                var approveOwner = productOffer.ApproveOffer(owner, ownersInStore.ToList());


                if (approveOwner != null)
                {
                    if (productOffer.State == ProductOfferState.Approved) 
                        await NotifyUserOnApprovedOffer(productOffer);

                    await _storeUnitOfWork.ApproversRepo.InsertOneAsync(approveOwner);
                    await _storeUnitOfWork.SaveAsync();
                }
            }
            catch (Exception e)
            {
                _logger.LogError("error");
                
            }
        }

        private Task NotifyUserOnApprovedOffer(ProductOffer offer)
        {
            var user = offer.User;


            var notifications = new List<(Guid, NotificationDto)>();

            var notification = new OfferApprovedNotification(offer);

            user.Notifications.Add(notification);
           
            notifications.Add((user.Guid, _mapper.Map<OfferApprovedNotificationDto>(notification)));
            _storeUnitOfWork.Attach(notification);


            return _notificationPublisher.NotifyManyAsync(notifications);
        }

        public async Task DeclineOffer(Guid ownerGuid, Guid productOfferGuid)
        {
            try
            {
                var offer = await _storeUnitOfWork.OffersRepo.FindByIdAsync(productOfferGuid);
                offer.State = ProductOfferState.Declined;
                await NotifyUserOnDeclinedOffer(offer);
                await _storeUnitOfWork.SaveAsync();
            }
            catch (Exception e)
            {
                _logger.LogError(e, "The following error occurred during decline product offer with guid {productOfferGuid}", productOfferGuid);
            }
        }

        private Task NotifyUserOnDeclinedOffer(ProductOffer offer)
        {
            var user = offer.User; 


            var notifications = new List<(Guid, NotificationDto)>();
           
                var notification = new OfferDeclinedNotification(offer);

                user.Notifications.Add(notification);
                notifications.Add((user.Guid, _mapper.Map<OfferDeclinedNotificationDto>(notification)));
                _storeUnitOfWork.Attach(notification);
            

            return _notificationPublisher.NotifyManyAsync(notifications);
        }

        public async Task<ProductOfferDto> MakeCounterOffer(Guid ownerGuid, decimal counterOfferPrice, Guid offerGuid)
        {
            try
            {
                var offer = await _storeUnitOfWork.OffersRepo.FindByIdAsync(offerGuid);
                if (!offer.MakeCounterOffer(counterOfferPrice))
                {
                    return null;
                }
                await _storeUnitOfWork.SaveAsync();

                return _mapper.Map<ProductOfferDto>(offer);
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"The following error occurred during make counter offer with guid {offerGuid}");
                return null;
            }
        }

        public async Task<ProductOfferDto> GetProductOffer(Guid offerGuid)
        {
            try
            {
                var productOffer = await _storeUnitOfWork.OffersRepo.FindByIdAsync(offerGuid);
                return _mapper.Map<ProductOfferDto>(productOffer);
            }
            catch (Exception e)
            {
                return null;
            }
        }

        public async Task<IEnumerable<ProductOfferDto>> GetAllUserProductOffers(Guid userGuid)
        {
            try
            {
                var productOffers = await _storeUnitOfWork.OffersRepo.FilterByAsync
                    (offer => offer.User.Guid == userGuid);

                return _mapper.Map<List<ProductOfferDto>>(productOffers.ToList());
            }
            catch (Exception e)
            {
                return null;
            }
        }

        public async Task<IEnumerable<ProductOfferDto>> GetAllOwnerProductOffers(Guid ownerGuid)
        {
            try
            {
                var owner = await _storeUnitOfWork.StoreOwnershipRepo.FindByIdAsync(ownerGuid);

                var productOffers = await _storeUnitOfWork.OffersRepo.FilterByAsync
                    (offer => offer.Product.Store.Guid == owner.Store.Guid);

                return _mapper.Map<List<ProductOfferDto>>(productOffers.ToList());
            }
            catch (Exception e)
            {
                return null;
            }
        }

    }

    
}
