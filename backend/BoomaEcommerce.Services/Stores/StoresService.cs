using BoomaEcommerce.Domain;
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
        private readonly IRepository<Store> _storeRepo;
        private readonly IRepository<StoreOwnership> _storeOwnershipRepo; 
        private readonly IRepository<StorePurchase> _storePurchaseRepo;
        
        public StoresService(ILogger<StoresService> logger,
            IMapper mapper,
            IRepository<Store> storeRepo,
            IRepository<StorePurchase> storePurchasesRepo, 
            IRepository<StoreOwnership> storeOwnershipRepo)
        {
            _logger = logger;
            _mapper = mapper;
            _storeRepo = storeRepo;
            _storePurchaseRepo = storePurchasesRepo;
            _storeOwnershipRepo = storeOwnershipRepo;
        }

        public async Task CreateStoreAsync(StoreDto store)
        {
            var newStore = _mapper.Map<Store>(store);
            try
            {
                await _storeRepo.InsertOneAsync(newStore); 
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message); 
            }

            var user = _mapper.Map<User>(newStore.StoreFounder);
            var storeOwnerShip = new StoreOwnership();
            storeOwnerShip.Store = newStore;
            storeOwnerShip.User = user; 

            try
            {
                await _storeOwnershipRepo.InsertOneAsync(storeOwnerShip); 
            }
            catch(Exception e)
            {
                _logger.LogError(e.Message);
            }
        }

        public async Task<IReadOnlyCollection<StoreDto>> GetStoresAsync()
        {
            try
            {
                _logger.LogInformation("Getting all stores.");
                var stores = await _storeRepo.FindAllAsync();
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
                var store = await _storeRepo.FindOneAsync(s => s.Guid == storeGuid);
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
                await _storeRepo.DeleteByIdAsync(storeGuid);
                return true;
            }
            catch (Exception e)
            {
                _logger.LogError($"Failed to delete store with guid {storeGuid}", e);
                return false;
            }
        }

        public async Task<IReadOnlyCollection<StorePurchaseDto>> GetStorePurchaseHistory(Guid storeGuid)
        {
            try
            {
                var purchaseHistory = await _storePurchaseRepo.FilterByAsync(p => p.Store.Guid == storeGuid);
                return _mapper.Map<IReadOnlyCollection<StorePurchaseDto>>(purchaseHistory.ToList());
            }
            catch(Exception e)
            {
                _logger.LogError(e.Message);
                return null;
            }
        }


    }
}
