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

        public async Task<IReadOnlyCollection<ProductDto>> GetProductsFromStoreAsync(Guid storeGuid)
        {
            try
            {
                _logger.LogInformation($"Getting products from UserStore with guid {storeGuid}");
                var products = await _productRepo.FilterByAsync(p => p.Store.Guid == storeGuid && !p.IsSoftDeleted);
                return _mapper.Map<IReadOnlyCollection<ProductDto>>(products.ToList());
            }
            catch (Exception e)
            {
                _logger.LogError($"Failed to get products from UserStore {storeGuid}", e);
                return null;
            }
        }


    }
}
