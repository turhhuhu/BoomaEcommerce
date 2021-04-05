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

        public StoresService(ILogger<StoresService> logger,
            IMapper mapper,
            IRepository<Store> storeRepo)
        {
            _logger = logger;
            _mapper = mapper;
            _storeRepo = storeRepo;
        }

        public async Task CreateStoreAsync(string userId, StoreDto store)
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

        public Task<IReadOnlyCollection<StoreDto>> GetAllStoresAsync()
        {
            throw new NotImplementedException();
        }

        public Task<StoreDto> GetStoreAsync(Guid storeGuid)
        {
            throw new NotImplementedException();
        }

        public Task DeleteStoreAsync(Guid storeGuid)
        {
            throw new NotImplementedException();
        }

        public Task<IReadOnlyCollection<StoreDto>> GetStoresAsync()
        {
            throw new NotImplementedException();
        }
    }
}
