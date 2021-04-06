using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using BoomaEcommerce.Data;
using BoomaEcommerce.Domain;
using BoomaEcommerce.Services.DTO;
using Microsoft.Extensions.Logging;

namespace BoomaEcommerce.Services.Products
{
    public class ProductsService : IProductsService
    {
        private readonly ILogger<ProductsService> _logger;
        private readonly IMapper _mapper;
        private readonly IRepository<Product> _productRepo;

        public ProductsService(ILogger<ProductsService> logger, IMapper mapper, IRepository<Product> productRepo)
        {
            _logger = logger;
            _mapper = mapper;
            _productRepo = productRepo;
        }
        public Task CreateStoreProductAsync(ProductDto product)
        {
            throw new NotImplementedException();
        }

        public Task<IReadOnlyCollection<ProductDto>> GetAllProductsAsync()
        {
            throw new NotImplementedException();
        }

        public async Task<IReadOnlyCollection<ProductDto>> GetProductsFromStoreAsync(Guid storeGuid)
        {
            try
            {
                var products = await _productRepo.FilterByAsync(p => p.Store.Guid == storeGuid);
                return _mapper.Map<IReadOnlyCollection<ProductDto>>(products.ToList());
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
                return null;
            }
        }

        public Task<ProductDto> GetProductAsync(Guid productGuid)
        {
            throw new NotImplementedException();
        }

        public Task DeleteProductAsync(Guid productGuid)
        {
            throw new NotImplementedException();
        }

        public Task UpdateProductAsync(ProductDto product)
        {
            throw new NotImplementedException();
        }
    }
}
