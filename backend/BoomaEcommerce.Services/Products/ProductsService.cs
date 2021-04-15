using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using BoomaEcommerce.Data;
using BoomaEcommerce.Domain;
using BoomaEcommerce.Services.DTO;
using BoomaEcommerce.Services.External;
using Microsoft.Extensions.Logging;


namespace BoomaEcommerce.Services.Products
{
    public class ProductsService : IProductsService
    {
        private readonly ILogger<ProductsService> _logger;
        private readonly IMapper _mapper;
        private readonly IRepository<Product> _productRepo;
        private readonly IMistakeCorrection _mistakeCorrection;

        public ProductsService(ILogger<ProductsService> logger, IMapper mapper, IRepository<Product> productRepo, IMistakeCorrection mistakeCorrection)
        {
            _logger = logger;
            _mapper = mapper;
            _productRepo = productRepo;
            _mistakeCorrection = mistakeCorrection;
        }
        public async Task<bool> CreateStoreProductAsync(ProductDto productDto)
        {
            try
            {
                var product = _mapper.Map<Product>(productDto);
                if (!product.ValidateStorePolicy() || !product.ValidateAmount())
                {
                    return false;
                }
                await _productRepo.InsertOneAsync(product);
                return true;
            }
            catch (Exception e)
            {
                _logger.LogError(e,"Failed to create product with Guid {ProductGuid}," +
                                   " for store with guid {StoreGuid}", productDto.Guid, productDto.Store.Guid);
                return false;
            }
        }

        public async Task<IReadOnlyCollection<ProductDto>> GetAllProductsAsync()
        {
            try
            {
                var products = await _productRepo.FindAllAsync();
                return _mapper.Map<IReadOnlyCollection<ProductDto>>(products);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Failed to get all products");
                return null;
            }
        }

        public async Task<IReadOnlyCollection<ProductDto>> GetProductsFromStoreAsync(Guid storeGuid)
        {
            try
            {
                _logger.LogInformation($"Getting products from store with guid {storeGuid}");
                var products = await _productRepo.FilterByAsync(p => p.Store.Guid == storeGuid && !p.IsSoftDeleted);
                return _mapper.Map<IReadOnlyCollection<ProductDto>>(products.ToList());
            }
            catch (Exception e)
            {
                _logger.LogError($"Failed to get products from store {storeGuid}", e);
                return null;
            }
        }

        public async Task<ProductDto> GetProductAsync(Guid productGuid)
        {
            try
            {
                _logger.LogInformation($"Getting product with guid {productGuid}");
                var product = await _productRepo.FindByIdAsync(productGuid);
                return product.IsSoftDeleted 
                    ? null 
                    : _mapper.Map<ProductDto>(product);
            }
            catch (Exception e)
            {
                _logger.LogError("Failed to get productDto", e);
                return null;
            }
        }

        public async Task<IReadOnlyCollection<ProductDto>> GetProductByNameAsync(string productName)
        {
            try
            {
                _logger.LogInformation($"Getting all products that have the name {productName}");
                var correctedProductName = _mistakeCorrection.CorrectMistakeIfThereIsAny(productName);
                var products =
                    await _productRepo.FilterByAsync(p => p.Name.Equals(correctedProductName) && !p.IsSoftDeleted);
                return _mapper.Map<IReadOnlyCollection<ProductDto>>(products.ToList());
            }
            catch (Exception e)
            {
                _logger.LogError($"Failed to get products that have the name {productName}", e);
                return null;
            }
        }
        
        public async Task<IReadOnlyCollection<ProductDto>> GetProductByCategoryAsync(string productCategory)
        {
            try
            {
                _logger.LogInformation($"Getting all products that have the category {productCategory}");
                var correctedProductCategory = _mistakeCorrection.CorrectMistakeIfThereIsAny(productCategory);
                var products =
                    await _productRepo.FilterByAsync(p => p.Category.Equals(correctedProductCategory) 
                                                          && !p.IsSoftDeleted);
                return _mapper.Map<IReadOnlyCollection<ProductDto>>(products.ToList());
            }
            catch (Exception e)
            {
                _logger.LogError($"Failed to get products that have the category {productCategory}", e);
                return null;
            }
        }
        
        public async Task<IReadOnlyCollection<ProductDto>> GetProductByKeywordAsync(string productKeyword)
        {
            try
            {
                _logger.LogInformation($"Getting all products that fit the keyword {productKeyword}");
                var correctedProductKeyWord = _mistakeCorrection.CorrectMistakeIfThereIsAny(productKeyword);
                var products =
                    await _productRepo.FilterByAsync(p => !p.IsSoftDeleted &&
                        (p.Category.Contains(correctedProductKeyWord) || p.Name.Contains(correctedProductKeyWord)));
                return _mapper.Map<IReadOnlyCollection<ProductDto>>(products.ToList());
            }
            catch (Exception e)
            {
                _logger.LogError($"Failed to get products that fit the keyword {productKeyword}", e);
                return null;
            }
        }

        public async Task<bool> DeleteProductAsync(Guid productGuid)
        {
            try
            {
                _logger.LogInformation($"Deleting product with guid {productGuid}");
                var product = await _productRepo.FindByIdAsync(productGuid);
                if (product == null) return false;
                if (product.IsSoftDeleted) return false;
                product.IsSoftDeleted = true;

                await _productRepo.ReplaceOneAsync(product);
                return true;
            }
            catch (Exception e)
            {
                _logger.LogError($"Failed to delete product with guid {productGuid}", e);
                return false;
            }
        }

        public async Task<bool> UpdateProductAsync(ProductDto productDto)
        {
            try
            {
                _logger.LogInformation($"Updating product with guid {productDto.Guid}");
                var product = await _productRepo.FindByIdAsync(productDto.Guid);
                if (product.IsSoftDeleted) return false;
                product.Name = productDto.Name ?? product.Name;
                product.Amount = productDto.Amount ?? product.Amount;
                product.Price = productDto.Price ?? product.Price;
                product.Category = productDto.Category ?? product.Category;

                await _productRepo.ReplaceOneAsync(product);
                return true;
            }
            catch (Exception e)
            {
                _logger.LogError($"Failed to update product with guid {productDto.Guid}", e);
                return false;
            }
        }
    }
}
