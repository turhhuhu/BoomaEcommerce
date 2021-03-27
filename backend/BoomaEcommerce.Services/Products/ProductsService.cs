using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BoomaEcommerce.Domain;
using BoomaEcommerce.Services.DTO;

namespace BoomaEcommerce.Services.Products
{
    public class ProductsService : IProductsService
    {
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
