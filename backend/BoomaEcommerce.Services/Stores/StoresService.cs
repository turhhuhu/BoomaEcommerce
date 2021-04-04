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
        private readonly IRepository<Purchases> _purchaesesRepo;
        public StoresService(ILogger<StoresService> logger,
            IMapper mapper,
            IRepository<Store> storeRepo,
            IRepository<Purchases> purchasesRepo)
        {
            _logger = logger;
            _mapper = mapper;
            _storeRepo = storeRepo;
            _purchaesesRepo = purchasesRepo;
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

        public Task<IReadOnlyCollection<PurchaseDto>> GetStorePurchaseHistory(Guid storeGuid)
        {

        }
    }
}
