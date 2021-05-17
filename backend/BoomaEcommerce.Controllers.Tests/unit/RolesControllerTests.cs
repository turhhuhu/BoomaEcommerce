using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using BoomaEcommerce.Api.Controllers;
using BoomaEcommerce.Api.Responses;
using BoomaEcommerce.Services;
using BoomaEcommerce.Services.DTO;
using BoomaEcommerce.Services.Stores;
using BoomaEcommerce.Services.Users;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

using BoomaEcommerce.Tests.CoreLib;



namespace BoomaEcommerce.Controllers.Tests.unit
{
    public class RolesControllerTests
    {

        private readonly RolesController _rolesController;
        private readonly Mock<IStoresService> _storeServicesMock;
        private readonly Guid _userGuidInClaims;
        private readonly IMapper _mapper = MapperFactory.GetMapper();


        public RolesControllerTests()
        {
            _storeServicesMock = new Mock<IStoresService>();
            _userGuidInClaims = Guid.NewGuid();
            
            _rolesController = new RolesController(_storeServicesMock.Object, _mapper);

            var fakeClaims = new List<Claim>
            {
                new("guid", _userGuidInClaims.ToString())
            };

            var fakeIdentity = new ClaimsIdentity(fakeClaims, "TestAuthType");
            var fakeClaimsPrincipal = new ClaimsPrincipal(fakeIdentity);

        }

        [Fact]
        public async Task GetOwnership_ShouldReturnOkResult_WhenStoreOwnershipExists()
        {
            // Arrange
            _storeServicesMock.Setup(x => x.GetStoreOwnershipAsync(It.IsAny<Guid>()))
                .ReturnsAsync((Guid storeOwnership) => new StoreOwnershipDto {Guid = storeOwnership});
            Guid storeOwnershipGuid = Guid.NewGuid();
            var expectedResult = new StoreOwnershipDto {Guid = storeOwnershipGuid};

            // Act
            var userInfoResult = await _rolesController.GetOwnership(storeOwnershipGuid);

            // Assert
            userInfoResult.Should().BeOfType<OkObjectResult>();
            var okResult = (OkObjectResult) userInfoResult;
            var res = (OwnerShipRoleResponse) okResult.Value;
            res.Guid.Should().Be(expectedResult.Guid);
        }
        
        [Fact]
        public async Task GetOwnership_ShouldReturnNotFound_WhenStoreOwnershipDoNotExists()
        {
            // Arrange
            _storeServicesMock.Setup(x => x.GetStoreOwnershipAsync(It.IsAny<Guid>()))
                .ReturnsAsync((Guid storeOwnership) => null);
            Guid storeOwnershipGuid = Guid.NewGuid();
            var expectedResult = new StoreOwnershipDto {Guid = storeOwnershipGuid};

            // Act
            var userInfoResult = await _rolesController.GetOwnership(storeOwnershipGuid);

            // Assert
            userInfoResult.Should().BeOfType<NotFoundResult>();

        }
        
        [Fact]
        public async Task GetManagement_ShouldReturnOkResult_WhenStoreManagementExists()
        {
            // Arrange
            _storeServicesMock.Setup(x => x.GetStoreManagementAsync(It.IsAny<Guid>()))
                .ReturnsAsync((Guid storeManagement) => new StoreManagementDto() {Guid = storeManagement});
            Guid storeManagementGuid = Guid.NewGuid();
            var expectedResult = new StoreOwnershipDto {Guid = storeManagementGuid};

            // Act
            var userInfoResult = await _rolesController.GetManagement(storeManagementGuid);

            // Assert
            userInfoResult.Should().BeOfType<OkObjectResult>();
            var okResult = (OkObjectResult) userInfoResult;
            var res = (ManagementRoleResponse) okResult.Value;
            res.Guid.Should().Be(expectedResult.Guid);
        }
        [Fact]
        public async Task GetManagement_ShouldReturnNotFound_WhenStoreManagementDoNotExists()
        {
            // Arrange
            _storeServicesMock.Setup(x => x.GetStoreManagementAsync(It.IsAny<Guid>()))
                .ReturnsAsync((Guid storeManagement) => null);
            Guid storeManagementGuid = Guid.NewGuid();
            var expectedResult = new StoreOwnershipDto {Guid = storeManagementGuid};

            // Act
            var userInfoResult = await _rolesController.GetManagement(storeManagementGuid);

            // Assert
            userInfoResult.Should().BeOfType<NotFoundResult>();

        }
        
        [Fact]
        public async Task UpdatePermissions_ShouldUpdatePermissions_WhenStoreManagementExists()
        {
            // Arrange
            var permissions = new StoreManagementPermissionsDto();
            Guid storeManagementGuid = Guid.NewGuid();

            // Act
            var userInfoResult = await _rolesController.UpdatePermissions(storeManagementGuid,permissions);

            // Assert
            userInfoResult.Should().BeOfType<NoContentResult>();
            
        }
        
        [Fact]
        public async Task GetSubordinates_ShouldReturnOkResult_WhenOwnerExists()
        {
            
            // Arrange
            _storeServicesMock.Setup(x => x.GetSubordinateSellersAsync(It.IsAny<Guid>(),It.IsAny<int?>()))
                .ReturnsAsync((Guid storeSellers,int? level) => new StoreSellersDto() {StoreOwners = new List<StoreOwnershipDto>()});
            
            Guid storeOwnershipGuid = Guid.NewGuid();
            var expectedResult = new List<StoreOwnershipDto>();

            // Act
            var userInfoResult = await _rolesController.GetSubordinates(storeOwnershipGuid);

            // Assert
            userInfoResult.Should().BeOfType<OkObjectResult>();
            var okResult = (OkObjectResult) userInfoResult;
            var res = (StoreSellersResponse) okResult.Value;
            res.StoreOwners.Should().BeEquivalentTo(expectedResult);
        }
        
        [Fact]
        public async Task GetSubordinates_ShouldReturnNotFound_WhenOwnerDoNotExists()
        {
            
            // Arrange
            _storeServicesMock.Setup(x => x.GetSubordinateSellersAsync(It.IsAny<Guid>(),It.IsAny<int?>()))
                .ReturnsAsync((Guid storeSellers,int? level) => null);
            
            Guid storeOwnershipGuid = Guid.NewGuid();

            // Act
            var userInfoResult = await _rolesController.GetSubordinates(storeOwnershipGuid);

            // Assert
            userInfoResult.Should().BeOfType<NotFoundResult>();
            
        }
    }
    
}