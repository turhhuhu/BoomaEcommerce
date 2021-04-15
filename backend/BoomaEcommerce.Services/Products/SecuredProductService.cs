using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security.Authentication;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using BoomaEcommerce.Data;
using BoomaEcommerce.Services.DTO;
using Microsoft.AspNetCore.Authorization;

namespace BoomaEcommerce.Services.Products
{
    public class SecuredProductService : SecuredServiceBase, IProductsService
    {
        private readonly IProductsService _productService;
        private readonly IAuthorizationService _authorizationService;
        public SecuredProductService(ClaimsPrincipal claimsPrincipal,
            IProductsService productService,
            IAuthorizationService authorizationService) 
            : base(claimsPrincipal)
        {
            _productService = productService;
            _authorizationService = authorizationService;
        }

        public Task<IReadOnlyCollection<ProductDto>> GetAllProductsAsync()
        {
            throw new NotImplementedException();
        }

        public Task<ProductDto> GetProductAsync(Guid productGuid)
        {
            throw new NotImplementedException();
        }

        public Task<IReadOnlyCollection<ProductDto>> GetProductByNameAsync(string productName)
        {
            throw new NotImplementedException();
        }

        public Task<IReadOnlyCollection<ProductDto>> GetProductByCategoryAsync(string productCategory)
        {
            throw new NotImplementedException();
        }

        public Task<IReadOnlyCollection<ProductDto>> GetProductByKeywordAsync(string productKeyword)
        {
            throw new NotImplementedException();
        }

        public Task<bool> DeleteProductAsync(Guid productGuid)
        {
            throw new NotImplementedException();
        }

        public Task<bool> UpdateProductAsync(ProductDto product)
        {
            throw new NotImplementedException();
        }
    }
}
