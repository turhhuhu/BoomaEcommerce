using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using BoomaEcommerce.Data;
using BoomaEcommerce.Domain;
using BoomaEcommerce.Services.DTO;
using BoomaEcommerce.Services.External;
using BoomaEcommerce.Services.MappingProfiles;
using BoomaEcommerce.Services.Products;
using BoomaEcommerce.Tests.CoreLib;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace BoomaEcommerce.Services.Tests
{

    public class ProductsServiceTests
    {
        private readonly Mock<ILogger<ProductsService>> _logger = new();
        private readonly IMapper _mapper = MapperFactory.GetMapper();

        private ProductsService GetProductService(IDictionary<Guid, Product> products)
        {
            var productRepoMock = DalMockFactory.MockRepository(products);
            var mistakeCorrectionMock = new Mock<IMistakeCorrection>();
            mistakeCorrectionMock.Setup(x => x.CorrectMistakeIfThereIsAny(It.IsAny<string>()))
                .Returns<string>(x => x);
            return new ProductsService(_logger.Object, _mapper, productRepoMock.Object, mistakeCorrectionMock.Object);
        }
        
        [Fact]
        public async Task GetProductsFromStoreAsync_ReturnsAllNotSoftDeletedProductsFromGivenStore_WhenStoreExists()
        {
            // Arrange
            var storeGuid = Guid.NewGuid();
            var notSoftDeletedProductGuidList = new List<Guid>();
            var productsDict = new Dictionary<Guid, Product>();
            for (var i = 0; i < 3; i++)
            {
                var notSoftDeletedProductGuid = Guid.NewGuid();
                var notSoftDeletedProduct = TestData.GetTestProductFromStore(notSoftDeletedProductGuid, storeGuid);
                productsDict.Add(notSoftDeletedProductGuid, notSoftDeletedProduct);
                notSoftDeletedProductGuidList.Add(notSoftDeletedProductGuid);
            }
            var softDeletedProductGuid = Guid.NewGuid();
            productsDict.Add(softDeletedProductGuid,
                new Product{Guid = softDeletedProductGuid, Store = new Store { Guid =  storeGuid}, IsSoftDeleted = true});
            
            var sut = GetProductService(productsDict);

            // Act
            var result = await sut.GetProductsFromStoreAsync(storeGuid);
    
            // Assert
            foreach (var productDto in result)
            {
                productDto.StoreGuid.Should().Be(storeGuid);
                notSoftDeletedProductGuidList.Should().Contain(productDto.Guid);
            }
            
        }
        
        [Fact]
        public async Task GetProductsFromStoreAsync_ReturnsEmptyCollection_WhenStoreDoesNotExists()
        {
            // Arrange
            var productsDict = new Dictionary<Guid, Product>();
            
            var sut = GetProductService(productsDict);
            
            // Act
            var result = await sut.GetProductsFromStoreAsync(Guid.NewGuid());

            // Assert
            result.Should().BeEmpty();
        }
        
        [Fact]
        public async Task GetProductAsync_ReturnsNotSafeDeletedProduct_WhenProductExistsAndNotSafeDeleted()
        {
            // Arrange
            var productsDict = new Dictionary<Guid, Product>();
            var productGuid = Guid.NewGuid();
            productsDict[productGuid] = TestData.GetTestProduct(productGuid);
            
            var sut = GetProductService(productsDict);

            // Act
            var result = await sut.GetProductAsync(productGuid);

            // Assert
            result.Guid.Should().Be(productGuid);
        }
        
        [Fact]
        public async Task GetProductAsync_ReturnsNull_WhenProductDoesNotExist()
        {
            // Arrange
            var productsDict = new Dictionary<Guid, Product>();
            
            var sut = GetProductService(productsDict);
                
            // Act
            var result = await sut.GetProductAsync(Guid.NewGuid());

            // Assert
            result.Should().BeNull();
        }
        
        [Fact]
        public async Task GetProductAsync_ReturnsNull_WhenProductExistsButIsSafeDeleted()
        {
            // Arrange
            var productsDict = new Dictionary<Guid, Product>();
            var productGuid = Guid.NewGuid();
            productsDict[productGuid] = new Product{Guid = productGuid, IsSoftDeleted = true};
            
            var sut = GetProductService(productsDict);

            // Act
            var result = await sut.GetProductAsync(Guid.NewGuid());

            // Assert
            result.Should().BeNull();
        }
        
        [Fact]
        public async Task GetProductByNameAsync_ReturnsAllNotSoftDeletedProducts_WhenProductExistsWithTheGivenNameAndNotSafeDeleted()
        {
            // Arrange
            var productsDict = new Dictionary<Guid, Product>();
            var productName = "ChessBoard";
            var notSoftDeletedProductGuidList = new List<Guid>();
            for (var i = 0; i < 3; i++)
            {
                var notSoftDeletedProductGuid = Guid.NewGuid();
                var notSoftDeletedProduct = 
                    new Product{Guid = notSoftDeletedProductGuid, Name = productName, IsSoftDeleted = false};
                productsDict.Add(notSoftDeletedProductGuid, notSoftDeletedProduct);
                notSoftDeletedProductGuidList.Add(notSoftDeletedProductGuid);
            }
            var softDeletedProductGuid = Guid.NewGuid();
            productsDict.Add(softDeletedProductGuid,
                new Product{Guid = softDeletedProductGuid, Name = productName, IsSoftDeleted = true});
            
            var sut = GetProductService(productsDict);

            // Act
            var result = await sut.GetProductByNameAsync(productName);
            
            // Assert
            foreach (var productDto in result)
            {
                productDto.Name.Should().Be(productName);
                notSoftDeletedProductGuidList.Should().Contain(productDto.Guid);
            }
        }
        
        [Fact]
        public async Task GetProductByNameAsync_ReturnsEmptyCollection_WhenProductDoesNotExists()
        {
            // Arrange
            var productsDict = new Dictionary<Guid, Product>();
            
            var sut = GetProductService(productsDict);

            // Act
            var result = await sut.GetProductByNameAsync("");

            // Assert
            result.Should().BeEmpty();
        }
        
        [Fact]
        public async Task GetProductByCategoryAsync_ReturnsAllNotSoftDeletedProducts_WhenProductExistsWithTheGivenCategoryAndNotSafeDeleted()
        {
            // Arrange
            var productsDict = new Dictionary<Guid, Product>();
            var productCategory = "Chess";
            var notSoftDeletedProductGuidList = new List<Guid>();
            for (var i = 0; i < 3; i++)
            {
                var notSoftDeletedProductGuid = Guid.NewGuid();
                var notSoftDeletedProduct = 
                    new Product{Guid = notSoftDeletedProductGuid, Category = productCategory, IsSoftDeleted = false};
                productsDict.Add(notSoftDeletedProductGuid, notSoftDeletedProduct);
                notSoftDeletedProductGuidList.Add(notSoftDeletedProductGuid);
            }
            var softDeletedProductGuid = Guid.NewGuid();
            productsDict.Add(softDeletedProductGuid,
                new Product{Guid = softDeletedProductGuid, Category = productCategory, IsSoftDeleted = true});
            
            var sut = GetProductService(productsDict);

            // Act
            var result = await sut.GetProductByCategoryAsync(productCategory);
            
            // Assert
            foreach (var productDto in result)
            {
                productDto.Category.Should().Be(productCategory);
                notSoftDeletedProductGuidList.Should().Contain(productDto.Guid);
            }
        }
        
        [Fact]
        public async Task GetProductByCategoryAsync_ReturnsEmptyCollection_WhenProductDoesNotExists()
        {
            // Arrange
            var productsDict = new Dictionary<Guid, Product>();
            
            var sut = GetProductService(productsDict);

            // Act
            var result = await sut.GetProductByCategoryAsync("");

            // Assert
            result.Should().BeEmpty();
        }
        
        [Fact]
        public async Task GetProductByKeywordAsync_ReturnsAllNotSoftDeletedProducts_WhenProductExistsWithTheGivenKeywordAndNotSafeDeleted()
        {
            // Arrange
            var productsDict = new Dictionary<Guid, Product>();
            const string productCategory = "Chess";
            const string productName = "ChessBoard";
            const string keyWord = "Chess";
            var notSoftDeletedProductGuidList = new List<Guid>();
            for (var i = 0; i < 3; i++)
            {
                var notSoftDeletedProductGuid = Guid.NewGuid();
                var notSoftDeletedProduct = 
                    new Product{Guid = notSoftDeletedProductGuid, Name = "checkers",
                        Category = productCategory, IsSoftDeleted = false};
                productsDict.Add(notSoftDeletedProductGuid, notSoftDeletedProduct);
                notSoftDeletedProductGuidList.Add(notSoftDeletedProductGuid);
            }
            for (var i = 0; i < 3; i++)
            {
                var notSoftDeletedProductGuid = Guid.NewGuid();
                var notSoftDeletedProduct = 
                    new Product{Guid = notSoftDeletedProductGuid, Name = productName,
                        Category = "checkers", IsSoftDeleted = false};
                productsDict.Add(notSoftDeletedProductGuid, notSoftDeletedProduct);
                notSoftDeletedProductGuidList.Add(notSoftDeletedProductGuid);
            }
            var softDeletedProductGuid = Guid.NewGuid();
            productsDict.Add(softDeletedProductGuid,
                new Product{Guid = softDeletedProductGuid, Category = productCategory, IsSoftDeleted = true});
            
            var sut = GetProductService(productsDict);

            // Act
            var result = await sut.GetProductByKeywordAsync(productCategory);
            
            // Assert
            foreach (var productDto in result)
            {
                productDto.Should().Match<ProductDto>(
                    x => x.Category.Contains(keyWord) || x.Name.Contains(keyWord));
                notSoftDeletedProductGuidList.Should().Contain(productDto.Guid);
            }
        }
        
        [Fact]
        public async Task GetProductByKeywordAsync_ReturnsEmptyCollection_WhenProductDoesNotExists()
        {
            // Arrange
            var productsDict = new Dictionary<Guid, Product>();
            
            var sut = GetProductService(productsDict);

            // Act
            var result = await sut.GetProductByKeywordAsync("");

            // Assert
            result.Should().BeEmpty();
        }

    }
}