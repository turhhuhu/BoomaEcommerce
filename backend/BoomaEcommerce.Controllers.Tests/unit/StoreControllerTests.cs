using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using BoomaEcommerce.Api.Controllers;
using BoomaEcommerce.Services;
using BoomaEcommerce.Services.DTO;
using BoomaEcommerce.Services.Stores;
using BoomaEcommerce.Services.Users;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace BoomaEcommerce.Controllers.Tests.unit
{
    public class StoreControllerTests
    {
        private readonly StoresController _storesController;
        private readonly Mock<IStoresService> _storeServicesMock;


        private readonly UsersController _usersController;
        private readonly Mock<IUsersService> _usersServiceMock;
        private readonly Guid _userGuidInClaims;

        public StoreControllerTests()
        {
            _usersServiceMock = new Mock<IUsersService>();
            _storeServicesMock = new Mock<IStoresService>();
            _storesController = new StoresController(_storeServicesMock.Object,null);
            _usersController = new UsersController(_usersServiceMock.Object, _storeServicesMock.Object, null, Mock.Of<INotificationPublisher>(), null);
            _userGuidInClaims = Guid.NewGuid();
            var fakeClaims = new List<Claim>
            {
                new("guid", _userGuidInClaims.ToString())
            };

            var fakeIdentity = new ClaimsIdentity(fakeClaims, "TestAuthType");
            var fakeClaimsPrincipal = new ClaimsPrincipal(fakeIdentity);
            _storesController.ControllerContext.HttpContext = new DefaultHttpContext
            {
                User = fakeClaimsPrincipal,
                Request =
                {
                    Scheme = "https",
                    Host = new HostString("localhost", 5001)
                }
            };
        }

        [Fact]
        public async Task CreateProduct_ShouldReturnCreatedProduct_WhenStoreServiceReturnsNotNullProduct()
        {
            // Arrange
            var storeGuid = Guid.NewGuid();
            var productGuid = Guid.NewGuid();
            var product = new ProductDto { Guid = productGuid ,  StoreGuid = storeGuid };

            _storeServicesMock.Setup(x => x.CreateStoreProductAsync(It.IsAny<ProductDto>()))
                .ReturnsAsync((ProductDto product) => 
                {
                    product.Guid = productGuid;
                 product.StoreGuid = storeGuid; 
                    return product;});

            var expectedResult = new ProductDto
            {
                Guid = productGuid,
                StoreGuid = storeGuid
            };

            // Act
            var productResult = await _storesController.CreateProduct(storeGuid,product);

            // Assert

            productResult.Should().BeOfType<CreatedResult>();
            var createdProductResult = (CreatedResult)productResult;
            createdProductResult.Location.Should().Be($"https://localhost:5001/products/{productGuid}");
            createdProductResult.Value.Should().BeEquivalentTo(expectedResult);
        }
        [Fact]
        public async Task CreateProduct_ShouldReturnStatusCodeInternalServerError_WhenStoreServiceReturnsNullProduct()
        {
            // Arrange
            _storeServicesMock.Setup(x => x.CreateStoreProductAsync(It.IsAny<ProductDto>()))
                            .ReturnsAsync((ProductDto product) => null);
            var storeGuid = Guid.NewGuid();
            var productGuid = Guid.NewGuid();
            var product = new ProductDto { Guid = productGuid, StoreGuid = storeGuid };

            // Act
            var productResult = await _storesController.CreateProduct(storeGuid, product);

            // Assert
            productResult.Should().BeOfType<BadRequestResult>();
        }
        [Fact]
        public async Task DeleteProduct_ShouldReturnTrue_WhenProductExists()
        {
            // Arrange
            _storeServicesMock.Setup(x => x.DeleteProductAsync(It.IsAny<Guid>()))
                .ReturnsAsync((Guid guid) => true);

            // Act
            var productResult = await _storesController.DeleteProduct(Guid.NewGuid());

            // Assert
            productResult.Should().BeOfType<NoContentResult>();
        }
        [Fact]
        public async Task DeleteProduct_ShouldReturnNotFoundResult_WhenStoreServiceReturnsFalse()
        {
            // Arrange
            _storeServicesMock.Setup(x => x.DeleteProductAsync(It.IsAny<Guid>()))
                .ReturnsAsync((Guid guid) => false);

            // Act
            var productResult = await _storesController.DeleteProduct(Guid.NewGuid());

            // Assert
            productResult.Should().BeOfType<NotFoundResult>();
        }

        //public async Task<IActionResult> UpdateProduct(Guid storeGuid, Guid productGuid, ProductDto product)
        [Fact]
        public async Task GetStoreProducts_ShouldReturnOkResult_WhenStoreExists()
        {
            // Arrange
            Guid storeGuid = Guid.NewGuid();
            Guid productGuid = Guid.NewGuid();
            _storeServicesMock.Setup(x => x.GetProductsFromStoreAsync(It.IsAny<Guid>()))
                .ReturnsAsync((Guid guid) =>
                {
                    List<ProductDto> products = new List<ProductDto>();
                    ProductDto p = new ProductDto { Guid = productGuid };
                    products.Add(p);
                    return products;
                });

            List<ProductDto> expectedResult = new List<ProductDto>();
            ProductDto p = new ProductDto { Guid = productGuid };
            expectedResult.Add(p);

            // Act
            var productsResult = await _storesController.GetStoreProducts(storeGuid);

            // Assert
            productsResult.Should().BeOfType<OkObjectResult>();
            var okResult = (OkObjectResult)productsResult;
            okResult.Value.Should().BeEquivalentTo(expectedResult);
        }
        [Fact]
        public async Task GetStoreProducts_ShouldReturnNotFoundResult_WhenStoreNotExists()
        {
            // Arrange
            Guid storeGuid = Guid.NewGuid();
            _storeServicesMock.Setup(x => x.GetProductsFromStoreAsync(It.IsAny<Guid>()))
                .ReturnsAsync((Guid guid) =>null);

            // Act
            var productsResult = await _storesController.GetStoreProducts(storeGuid);

            // Assert
            productsResult.Should().BeOfType<NotFoundResult>();
        }
        [Fact]
        public async Task GetStoreProduct_ShouldReturnOkResult_WhenProductExists()
        {
            // Arrange
            Guid productGuid = Guid.NewGuid();
            _storeServicesMock.Setup(x => x.GetStoreProductAsync(It.IsAny<Guid>()))
                .ReturnsAsync((Guid guid) => new ProductDto { Guid = guid });
            var expectedResult = new ProductDto { Guid = productGuid };

            // Act
            var productResult = await _storesController.GetStoreProduct(productGuid);

            // Assert
            productResult.Should().BeOfType<OkObjectResult>();
            var okResult = (OkObjectResult)productResult;
            okResult.Value.Should().BeEquivalentTo(expectedResult);
        }
        [Fact]
        public async Task GetStoreProduct_ShouldReturnNotFoundResult_WhenProductNotExists()
        {
            // Arrange
            Guid productGuid = Guid.NewGuid();
            _storeServicesMock.Setup(x => x.GetStoreProductAsync(It.IsAny<Guid>()))
                .ReturnsAsync((Guid guid) =>null);

            // Act
            var productResult = await _storesController.GetStoreProduct(productGuid);

            // Assert
            productResult.Should().BeOfType<NotFoundResult>();
        }
        [Fact]
        public async Task GetStore_ShouldReturnOkResult_WhenStoreExists()
        {
            // Arrange
            Guid storeGuid = Guid.NewGuid();
            _storeServicesMock.Setup(x => x.GetStoreAsync(It.IsAny<Guid>()))
                .ReturnsAsync((Guid guid) => new StoreDto { Guid = guid });
            var expectedResult = new StoreDto { Guid = storeGuid };

            // Act
            var storeResult = await _storesController.GetStore(storeGuid);

            // Assert
            storeResult.Should().BeOfType<OkObjectResult>();
            var okResult = (OkObjectResult)storeResult;
            okResult.Value.Should().BeEquivalentTo(expectedResult);
        }
        [Fact]
        public async Task GetStore_ShouldReturnNotFoundResult_WhenStoreNotExists()
        {
            // Arrange
            Guid storeGuid = Guid.NewGuid();
            _storeServicesMock.Setup(x => x.GetStoreAsync(It.IsAny<Guid>()))
                .ReturnsAsync((Guid guid) => null);

            // Act
            var storeResult = await _storesController.GetStore(storeGuid);

            // Assert
            storeResult.Should().BeOfType<NotFoundResult>();
        }
        [Fact]
        public async Task GetStores_ShouldReturnOkResult_WhenStoresExists()
        {
            // Arrange
            Guid store1Guid = Guid.NewGuid();
            Guid store2Guid = Guid.NewGuid();

            _storeServicesMock.Setup(x => x.GetStoresAsync())
                .ReturnsAsync(() =>
                {
                    List<StoreDto> stores = new List<StoreDto>();
                    StoreDto s1 = new StoreDto { Guid = store1Guid };
                    StoreDto s2 = new StoreDto { Guid = store2Guid };

                    stores.Add(s1);
                    stores.Add(s2);
                    return stores;
                });

            List<StoreDto> expectedResult = new List<StoreDto>();
            StoreDto s1 = new StoreDto { Guid = store1Guid };
            StoreDto s2 = new StoreDto { Guid = store2Guid };

            expectedResult.Add(s1);
            expectedResult.Add(s2);

            // Act
            var storeResult = await _storesController.GetStores();

            // Assert
            storeResult.Should().BeOfType<OkObjectResult>();
            var okResult = (OkObjectResult)storeResult;
            okResult.Value.Should().BeEquivalentTo(expectedResult);
        }
        [Fact]
        public async Task GetStores_ShouldReturnStatusCodeInternalServerError_WhenStoresNotExists()
        {
            // Arrange
            _storeServicesMock.Setup(x => x.GetStoresAsync())
                .ReturnsAsync(() => null);

            // Act
            var storeResult = await _storesController.GetStores();

            // Assert
            storeResult.Should().BeOfType<StatusCodeResult>();
            var statusCodeResult = (StatusCodeResult)storeResult;
            statusCodeResult.StatusCode.Should().Be(StatusCodes.Status500InternalServerError);
        }

        //public async Task<IActionResult> GetStoreRoles(Guid storeGuid)
        //public async Task<IActionResult> PostOwnershipRole(Guid storeGuid, [FromBody] CreateOwnershipRequest request)//public async Task<IActionResult> PostOwnershipRole(Guid storeGuid, [FromBody] CreateManagementRequest request)
        //public async Task<IActionResult> GetSubordinates(Guid ownershipGuid)




    }
}
