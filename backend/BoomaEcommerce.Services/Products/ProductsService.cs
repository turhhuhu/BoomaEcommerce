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
        public Task CreateStoreProductAsync(ProductDto product)
        {
            throw new NotImplementedException();
        }

        public Task GetProductsFromStore(Guid storeGuid)
        {
            var products = _
        }

        public Task<IReadOnlyCollection<ProductDto>> GetAllProductsAsync()
        {
            throw new NotImplementedException();
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
