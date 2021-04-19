using BoomaEcommerce.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BoomaEcommerce.Services.DTO;

namespace BoomaEcommerce.Services.Products
{
    public interface IProductsService
    {

        /// <summary>
        /// Gets all products.
        /// </summary>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the products collection.
        /// </returns>
        Task<IReadOnlyCollection<ProductDto>> GetAllProductsAsync();

        /// <summary>
        /// Get a product by guid.
        /// </summary>
        /// <param name="productGuid"></param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the product.
        /// </returns>
        Task<ProductDto> GetProductAsync(Guid productGuid);
        
        /// <summary>
        /// Get all products that have the given name
        /// </summary>
        /// <param name="productName"></param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the products collection.
        /// </returns>
        Task<IReadOnlyCollection<ProductDto>> GetProductByNameAsync(string productName);
        
        /// <summary>
        /// Get all products that have the given category
        /// </summary>
        /// <param name="productCategory"></param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the products collection.
        /// </returns>
        Task<IReadOnlyCollection<ProductDto>> GetProductByCategoryAsync(string productCategory);
        
        /// <summary>
        /// Get all products that have the given category
        /// </summary>
        /// <param name="productKeyword"></param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the products collection.
        /// </returns>
        Task<IReadOnlyCollection<ProductDto>> GetProductByKeywordAsync(string productKeyword);
    }
}
