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

        public StoresService(ILogger<StoresService> logger,
            IMapper mapper,
            IRepository<Store> storeRepo)
        {
            _logger = logger;
            _mapper = mapper;
            _storeRepo = storeRepo;
        }

        public Task CreateStoreAsync(string userId, StoreDto store)
        {
            throw new NotImplementedException();
        }

        public Task<IReadOnlyCollection<StoreDto>> GetAllStoresAsync()
        {
            throw new NotImplementedException();
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

        public Task DeleteStoreAsync(Guid storeGuid)
        {
            throw new NotImplementedException();
        }

        public async Task<IReadOnlyCollection<StoreDto>> GetStoresAsync()
        {
            try
            {
                var stores = await _storeRepo.FilterByAsync(s => true);
                return _mapper.Map<IReadOnlyCollection<StoreDto>>(stores.ToList());
            }
            catch(Exception e)
            {
                _logger.LogError(e.Message);
                return null; 
            }
        }
    }
}
