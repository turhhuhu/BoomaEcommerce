using BoomaEcommerce.Domain;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using BoomaEcommerce.Services.DTO;

namespace BoomaEcommerce.Services.Stores
{
    public class StoresService : IStoresService
    {
        private readonly ILogger<StoresService> _logger;
        private readonly IMapper _mapper;

        public StoresService(ILogger<StoresService> logger, IMapper mapper)
        {
            _logger = logger;
            _mapper = mapper;
        }

        public Task CreateStoreProductAsync(ProductDto product)
        {
            throw new NotImplementedException();
        }

        public Task<IReadOnlyCollection<StoreDto>> GetAllStoreAsync()
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
