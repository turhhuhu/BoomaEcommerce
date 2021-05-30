using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using BoomaEcommerce.Api.Controllers;
using BoomaEcommerce.Services;
using BoomaEcommerce.Services.DTO;
using BoomaEcommerce.Services.Products;
using BoomaEcommerce.Services.Stores;
using BoomaEcommerce.Services.Users;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace BoomaEcommerce.Controllers.Tests.unit
{
    public class ProductsControllerTests
    {
        // public ProductsController(IProductsService productService, IStoresService storesService)

        private readonly ProductsController _productsController;
        private readonly Mock<IProductsService> _productsServiceMock;
        private readonly Mock<IStoresService> _storeServicesMock;

        public ProductsControllerTests()
        {
            _storeServicesMock = new Mock<IStoresService>();
            _productsServiceMock = new Mock<IProductsService>();
            _productsController = new ProductsController(_productsServiceMock.Object, _storeServicesMock.Object);

            _productsController.ControllerContext.HttpContext = new DefaultHttpContext
            {
                Request =
                {
                    Scheme = "https",
                    Host = new HostString("localhost", 5001)
                }
            };
        }

        // public async Task<IActionResult> GetProduct(Id productGuid) 
        [Fact]
        public async Task GetProduct_ShouldReturnOkResult_WhenProductExists()
        {
            // Arrange

            Guid newProductGuid = Guid.NewGuid();
            _productsServiceMock.Setup(productsService => productsService.GetProductAsync(It.IsAny<Guid>()))
                .ReturnsAsync((Guid guid) => new ProductDto { Guid = guid });
            var expectedResult = new ProductDto { Guid = newProductGuid };

            // Act
            var productsRes = await _productsController.GetProduct(newProductGuid);

            // Assert
            productsRes.Should().BeOfType<OkObjectResult>();
            var okResult = (OkObjectResult)productsRes;
            okResult.Value.Should().BeEquivalentTo(expectedResult);
        }

        [Fact]
        public async Task GetProduct_ShouldReturnNotFoundStatus_WhenProductDoesNOTExists()
        {
            // Arrange
            _productsServiceMock.Setup(productsService => productsService.GetProductAsync(It.IsAny<Guid>()))
                .ReturnsAsync((Guid _) => null);

            // Act
            var productsRes = await _productsController.GetProduct(Guid.NewGuid());

            // Assert
            productsRes.Should().BeOfType<NotFoundResult>();
        }

        // public async Task<IActionResult> GetProducts
        [Fact]
        public async Task GetProducts_ShouldReturnOkResult_WhenKeywordExists()
        {
            // Arrange
            _productsServiceMock.Setup(productsService => productsService.GetProductByKeywordAsync(It.IsAny<string>()))
                .ReturnsAsync((string kw) =>
                {
                    List<ProductDto> retList = new List<ProductDto>();
                    retList.Add(new ProductDto {Category = kw});
                    return retList;
                });
            string exampleKW = "Balloon";
            List<ProductDto> expectedList = new List<ProductDto>();
            expectedList.Add(new ProductDto { Category = exampleKW });

            // Act
            var productsRes = await _productsController.GetProducts("Balloon", null, null, null);

            // Assert
            productsRes.Should().BeOfType<OkObjectResult>();
            var okResult = (OkObjectResult)productsRes;
            okResult.Value.Should().BeEquivalentTo(expectedList);
        }

        [Fact]
        public async Task GetProducts_ShouldReturnOkResult_WhenKeywordDoesNOTExitsButCategoryExists()
        {
            // Arrange
            _productsServiceMock.Setup(productsService => 
                    productsService.GetAllProductsAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<decimal?>()))
                .ReturnsAsync((string category, string name, decimal rate) =>
                {
                    List<ProductDto> retList = new List<ProductDto>();
                    retList.Add(new ProductDto { Category = category });
                    return retList;
                });

            string exampleCategory = "Foodies";
            List<ProductDto> expectedList = new List<ProductDto>();
            expectedList.Add(new ProductDto { Category = exampleCategory });

            // Act
            var productsRes = await _productsController.GetProducts(null, exampleCategory, null, null);

            // Assert
            productsRes.Should().BeOfType<OkObjectResult>();
            var okResult = (OkObjectResult)productsRes;
            okResult.Value.Should().BeEquivalentTo(expectedList);
        }

        [Fact]
        public async Task GetProducts_ShouldReturn500StatusCodeResult_WhenKeywordDoesNOTExitsANDInvalidDetails()
        {
            // Arrange
            _productsServiceMock.Setup(productsService =>
                    productsService.GetAllProductsAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<decimal?>()))
                .ReturnsAsync((string category, string name, decimal rate) => null);

            string exampleCategory = "Foodies";
            List<ProductDto> expectedList = new List<ProductDto>();
            expectedList.Add(new ProductDto { Category = exampleCategory });

            // Act
            var productsRes = await _productsController.GetProducts(null, exampleCategory, null, null);

            // Assert
            productsRes.Should().BeOfType<StatusCodeResult>();
            var res = (StatusCodeResult) productsRes;
            res.StatusCode.Should().Be(500);
        }

        [Fact]
        public async Task GetProducts_ShouldReturn500StatusCodeResult_WhenKeywordExitsButInvalidDetails()
        {
            // Arrange
            _productsServiceMock.Setup(productsService => productsService.GetProductByKeywordAsync(It.IsAny<string>()))
                .ReturnsAsync((string kw) => null);

            string exampleKW = "Balloon";
            List<ProductDto> expectedList = new List<ProductDto>();
            expectedList.Add(new ProductDto { Name = exampleKW });

            // Act
            var productsRes = await _productsController.GetProducts(exampleKW, null, null, null);

            // Assert
            productsRes.Should().BeOfType<StatusCodeResult>();
            var res = (StatusCodeResult)productsRes;
            res.StatusCode.Should().Be(500);
        }

        // public async Task<IActionResult> CreateProduct([FromBody] ProductDto product)
        [Fact]
        public async Task CreateProduct_ShouldReturnCreatedProduct_WhenProductDTOIsValid()
        {
            // Arrange
            Guid guid = Guid.NewGuid();
            _storeServicesMock.Setup(ss => ss.CreateStoreProductAsync(It.IsAny<ProductDto>()))
                .ReturnsAsync((ProductDto pDTO) => pDTO);
            var expectedResult = new ProductDto
            {
                Guid = guid
            };

            // Act
            var productResult = await _productsController.CreateProduct(expectedResult);

            // Assert

            productResult.Should().BeOfType<CreatedResult>();
            var createdStoreResult = (CreatedResult)productResult;
            createdStoreResult.Location.Should().Be($"https://localhost:5001/products/{guid}");
            createdStoreResult.Value.Should().BeEquivalentTo(expectedResult);
        }

        [Fact]
        public async Task CreateProduct_ShouldReturnBadRequestCode_WhenCreateFunctionReturnsNull()
        {
            // Arrange
            Guid guid = Guid.NewGuid();
            _storeServicesMock.Setup(ss => ss.CreateStoreProductAsync(It.IsAny<ProductDto>()))
                .ReturnsAsync((ProductDto pDTO) => pDTO);
            var expectedResult = new ProductDto
            {
                Guid = guid
            };

            // Act
            var productResult = await _productsController.CreateProduct(expectedResult);

            // Assert

            productResult.Should().BeOfType<CreatedResult>();
            var createdStoreResult = (CreatedResult)productResult;
            createdStoreResult.Location.Should().Be($"https://localhost:5001/products/{guid}");
            createdStoreResult.Value.Should().BeEquivalentTo(expectedResult);
        }

        // public async Task<IActionResult> DeleteProduct(Id productGuid)

        [Fact]
        public async Task DeleteProduct_ShouldReturnNoContentResult_WhenProductExists()
        {
            // Arrange
            _storeServicesMock.Setup(ss => ss.DeleteProductAsync(It.IsAny<Guid>()))
                .ReturnsAsync(true);

            // Act
            var productsRes = await _productsController.DeleteProduct(Guid.NewGuid());

            // Assert
            productsRes.Should().BeOfType<NoContentResult>();
        }

        [Fact]
        public async Task DeleteProduct_ShouldReturnNotFoundResult_WhenProductDoesNOTExist()
        {
            // Arrange

            Guid newProductGuid = Guid.NewGuid();
            _storeServicesMock.Setup(ss => ss.DeleteProductAsync(It.IsAny<Guid>()))
                .ReturnsAsync(false);

            // Act
            var productsRes = await _productsController.DeleteProduct(newProductGuid);

            // Assert
            productsRes.Should().BeOfType<NotFoundResult>();
        }

        //   public async Task<IActionResult> UpdateProduct(Id productGuid, ProductDto product)

        [Fact]
        public async Task UpdateProduct_ShouldReturnNoContentResult_WhenProductExists()
        {
            // Arrange
            _storeServicesMock.Setup(ss => ss.UpdateProductAsync(It.IsAny<ProductDto>()))
                .ReturnsAsync(true);

            // Act
            var productsRes = await _productsController.UpdateProduct(Guid.NewGuid(), new ProductDto());

            // Assert
            productsRes.Should().BeOfType<NoContentResult>();
        }

        [Fact]
        public async Task UpdateProduct_ShouldReturnNotFoundResult_WhenProductDoesNOTExist()
        {
            // Arrange

            // Arrange
            _storeServicesMock.Setup(ss => ss.UpdateProductAsync(It.IsAny<ProductDto>()))
                .ReturnsAsync(false);

            // Act
            var productsRes = await _productsController.UpdateProduct(Guid.NewGuid(), new ProductDto());

            // Assert
            productsRes.Should().BeOfType<NotFoundResult>();
        }
    }
}
