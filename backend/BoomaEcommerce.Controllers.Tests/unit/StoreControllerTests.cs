using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using BoomaEcommerce.Api.Controllers;
using BoomaEcommerce.Api.Requests;
using BoomaEcommerce.Api.Responses;
using BoomaEcommerce.Domain;
using BoomaEcommerce.Services;
using BoomaEcommerce.Services.DTO;
using BoomaEcommerce.Services.Stores;
using BoomaEcommerce.Services.Users;
using BoomaEcommerce.Tests.CoreLib;
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
        private readonly IMapper _mapper = MapperFactory.GetMapper();

        public StoreControllerTests()
        {
            _usersServiceMock = new Mock<IUsersService>();
            _storeServicesMock = new Mock<IStoresService>();
            _storesController = new StoresController(_storeServicesMock.Object, _mapper, _usersServiceMock.Object);
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
            var product = new ProductDto { Guid = productGuid, StoreGuid = storeGuid };

            _storeServicesMock.Setup(x => x.CreateStoreProductAsync(It.IsAny<ProductDto>()))
                .ReturnsAsync((ProductDto product) =>
                {
                    product.Guid = productGuid;
                    product.StoreGuid = storeGuid;
                    return product;
                });

            var expectedResult = new ProductDto
            {
                Guid = productGuid,
                StoreGuid = storeGuid
            };

            // Act
            var productResult = await _storesController.CreateProduct(storeGuid, product);

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
        [Fact]
        public async Task UpdateProduct_ShouldReturnNoContent_WhenStoreServiceReturnsTrue()
        {
            // Arrange
            var storeGuid = Guid.NewGuid();
            var productGuid = Guid.NewGuid();
            var product = new ProductDto { Guid = productGuid, StoreGuid = storeGuid };

            _storeServicesMock.Setup(x => x.UpdateProductAsync(It.IsAny<ProductDto>()))
                .ReturnsAsync((ProductDto product) => true);


            // Act
            var productResult = await _storesController.UpdateProduct(storeGuid, productGuid, product);

            // Assert
            productResult.Should().BeOfType<NoContentResult>();
        }
        [Fact]
        public async Task UpdateProduct_ShouldReturnNotFound_WhenStoreServiceReturnsFalse()
        {
            // Arrange
            var storeGuid = Guid.NewGuid();
            var productGuid = Guid.NewGuid();
            var product = new ProductDto { Guid = productGuid, StoreGuid = storeGuid };

            _storeServicesMock.Setup(x => x.UpdateProductAsync(It.IsAny<ProductDto>()))
                .ReturnsAsync((ProductDto product) => false);

            // Act
            var productResult = await _storesController.UpdateProduct(storeGuid, productGuid, product);

            // Assert
            productResult.Should().BeOfType<NotFoundResult>();
        }
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
                .ReturnsAsync((Guid guid) => null);

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
                .ReturnsAsync((Guid guid) => null);

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
        [Fact]
        public async Task GetStoreRoles_ShouldReturnOk_WhenStoreServiceReturnsNotNullObject()
        {
            // Arrange
            var storeGuid = Guid.NewGuid();
            var storeMangmentGuid = Guid.NewGuid();
            StoreManagementDto storeManagement = new() { Guid = storeMangmentGuid };
            var storeMangmentList = new List<StoreManagementDto>();
            storeMangmentList.Add(storeManagement);
            StoreSellersDto storeSellers = new() { StoreManagers = storeMangmentList };
            _storeServicesMock.Setup(x => x.GetAllSellersInformationAsync(It.IsAny<Guid>()))
                .ReturnsAsync((Guid storeGuid) => storeSellers);


            // Act
            var storeRolesResult = await _storesController.GetStoreRoles(storeGuid);

            // Assert

            storeRolesResult.Should().BeOfType<OkObjectResult>();
            var createdstoreRolesResult = (OkObjectResult)storeRolesResult;
            createdstoreRolesResult.Value.Should().BeEquivalentTo(_mapper.Map<StoreSellersResponse>(storeSellers));
        }
        [Fact]
        public async Task GetStoreRoles_ShouldReturnInternalServerError_WhenStoreServiceReturnsNullObject()
        {
            // Arrange
            var storeGuid = Guid.NewGuid();
            var storeMangmentGuid = Guid.NewGuid();
            StoreManagementDto storeManagement = new() { Guid = storeMangmentGuid };
            var storeMangmentList = new List<StoreManagementDto>();
            storeMangmentList.Add(storeManagement);
            StoreSellersDto storeSellers = new() { StoreManagers = storeMangmentList };
            _storeServicesMock.Setup(x => x.GetAllSellersInformationAsync(It.IsAny<Guid>()))
                .ReturnsAsync((Guid storeGuid) => null);

            // Act
            var storeRolesResult = await _storesController.GetStoreRoles(storeGuid);

            // Assert

            storeRolesResult.Should().BeOfType<StatusCodeResult>();
            var createdstoreRolesResult = (StatusCodeResult)storeRolesResult;
            createdstoreRolesResult.StatusCode.Should().Be(StatusCodes.Status500InternalServerError);
        }
        [Fact]
        public async Task GetSubordinates_ShouldReturnOkResult_WhenOwnerExists()
        {

            // Arrange
            _storeServicesMock.Setup(x => x.GetSubordinateSellersAsync(It.IsAny<Guid>(), It.IsAny<int?>()))
                .ReturnsAsync((Guid storeSellers, int? level) => new StoreSellersDto() { StoreOwners = new List<StoreOwnershipDto>() });

            Guid storeOwnershipGuid = Guid.NewGuid();
            var expectedResult = new List<StoreOwnershipDto>();

            // Act
            var userInfoResult = await _storesController.GetSubordinates(storeOwnershipGuid);

            // Assert
            userInfoResult.Should().BeOfType<OkObjectResult>();
            var okResult = (OkObjectResult)userInfoResult;
            var res = (StoreSellersResponse)okResult.Value;
            res.StoreOwners.Should().BeEquivalentTo(expectedResult);
        }
        [Fact]
        public async Task GetSubordinates_ShouldReturnNotFound_WhenOwnerDoNotExists()
        {

            // Arrange
            _storeServicesMock.Setup(x => x.GetSubordinateSellersAsync(It.IsAny<Guid>(), It.IsAny<int?>()))
                .ReturnsAsync((Guid storeSellers, int? level) => null);

            Guid storeOwnershipGuid = Guid.NewGuid();

            // Act
            var userInfoResult = await _storesController.GetSubordinates(storeOwnershipGuid);

            // Assert
            userInfoResult.Should().BeOfType<NotFoundResult>();

        }
        [Fact]
        public async Task PostOwnershipRole_ShouldReturnOkResult_WhenStoreServiceReturnTrue()
        {
            //Arrange 
            _storeServicesMock.Setup(x => x.NominateNewStoreOwnerAsync(It.IsAny<Guid>(), It.IsAny<StoreOwnershipDto>()))
                         .ReturnsAsync((Guid storeSellers, StoreOwnershipDto s) => true);
            CreateOwnershipRequest c = new() { NominatedUserGuid = Guid.NewGuid() };
            //Act
            var userInfoResult = await _storesController.PostOwnershipRole(Guid.NewGuid(), c);

            //Assert
            userInfoResult.Should().BeOfType<OkResult>();

        }
        [Fact]
        public async Task PostOwnershipRole_ShouldReturnBadRequest_WhenStoreServiceReturnFalse()
        {
            //Arrange 
            _storeServicesMock.Setup(x => x.NominateNewStoreOwnerAsync(It.IsAny<Guid>(), It.IsAny<StoreOwnershipDto>()))
                         .ReturnsAsync((Guid storeSellers, StoreOwnershipDto s) => false);
            CreateOwnershipRequest c = new() { NominatedUserGuid = Guid.NewGuid() };
            //Act
            var userInfoResult = await _storesController.PostOwnershipRole(Guid.NewGuid(), c);

            //Assert
            userInfoResult.Should().BeOfType<BadRequestResult>();

        }
    }
}
