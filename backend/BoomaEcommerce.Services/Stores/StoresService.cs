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
        private readonly IRepository<StorePurchase> _storePurchaseRepo;
        public StoresService(ILogger<StoresService> logger,
            IMapper mapper,
            IRepository<Store> storeRepo,
            IRepository<StorePurchase> storePurchasesRepo)
        {
            _logger = logger;
            _mapper = mapper;
            _storeRepo = storeRepo;
            _storePurchaseRepo = storePurchasesRepo;
        }

        public Task CreateStoreAsync(string userId, StoreDto store)
        {
            throw new NotImplementedException();
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

        public async Task<IReadOnlyCollection<StorePurchaseDto>> GetStorePurchaseHistory(Guid storeGuid)
        {
            try
            {
                var purchaseHistory = await _storePurchaseRepo.FilterByAsync(p => p.Store.Guid == storeGuid);
                return _mapper.Map<IReadOnlyCollection<StorePurchaseDto>>(purchaseHistory);
            }
            catch(Exception e)
            {
                _logger.LogError(e.Message);
                return null;
            }
        }

     
    }
}
