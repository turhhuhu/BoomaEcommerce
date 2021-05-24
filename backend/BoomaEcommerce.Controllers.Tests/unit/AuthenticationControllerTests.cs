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
using BoomaEcommerce.Services;
using BoomaEcommerce.Services.Authentication;
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
    public class AuthenticationControllerTests
    {
        private readonly AuthenticationController _authenticationController;
        private readonly Mock<IAuthenticationService> _authServiceMock;

        public AuthenticationControllerTests()
        {
            _authServiceMock = new Mock<IAuthenticationService>();
            _authenticationController = new AuthenticationController(_authServiceMock.Object);

            _authenticationController.ControllerContext.HttpContext = new DefaultHttpContext
            {
                Request =
                {
                    Scheme = "https",
                    Host = new HostString("localhost", 5001)
                }
            };
        }

        // public async Task<ActionResult> Register([FromBody] UserRegistrationRequest request)

        [Fact]
        public async Task Register_ShouldReturnOkResult_WhenRegistrationRequestIsValid()
        {

            // Arrange
            var userGuid = Guid.NewGuid();
            var userInfo = new UserDto {Guid = userGuid};
            _authServiceMock.Setup(x => x.RegisterAsync(It.IsAny<UserDto>(), It.IsAny<string>()))
                .ReturnsAsync((UserDto u, string password) => new AuthenticationResult {Success = true, UserGuid = userGuid});

            var expectedResult = new SuccessAuthResponse()
            {
                UserGuid = userGuid
            };
            

            // Act
            var result = await _authenticationController.
                Register(new UserRegistrationRequest {UserInfo = userInfo, Password = "Pass"});

            // Assert

            result.Should().BeOfType<OkObjectResult>();
            var resObj = (OkObjectResult) result;
            resObj.Value.Should().BeEquivalentTo(expectedResult);
        }

        [Fact]
        public async Task Register_ShouldReturnBadRequest_WhenRegistrationRequestIsNOTValid()
        {

            // Arrange
            var userGuid = Guid.NewGuid();
            var userInfo = new UserDto { Guid = userGuid };
            _authServiceMock.Setup(x => x.RegisterAsync(It.IsAny<UserDto>(), It.IsAny<string>()))
                .ReturnsAsync((UserDto u, string password) => new AuthenticationResult { Success = false });


            // Act
            var result = await _authenticationController.
                Register(new UserRegistrationRequest { UserInfo = userInfo, Password = "Pass" });

            // Assert

            result.Should().BeOfType<BadRequestObjectResult>();
        }


        // public async Task<ActionResult> Login([FromBody] UserLoginRequest request)

        [Fact]
        public async Task Login_ShouldReturnOkResult_WhenLoginRequestIsValid()
        {

            // Arrange
            var userGuid = Guid.NewGuid();
            string username = "Benny"; 
            string password = "secret";
            _authServiceMock.Setup(x => x.LoginAsync(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync((string username, string password) => new AuthenticationResult { Success = true, UserGuid = userGuid });

            var expectedResult = new SuccessAuthResponse()
            {
                UserGuid = userGuid
            };


            // Act
            var result = await _authenticationController.
                Login(new UserLoginRequest { Username = username, Password = password });

            // Assert

            result.Should().BeOfType<OkObjectResult>();
            var resObj = (OkObjectResult)result;
            resObj.Value.Should().BeEquivalentTo(expectedResult);
        }




        [Fact]
        public async Task Login_ShouldReturnBadRequest_WhenLoginRequestIsNOTValid()
        {

            // Arrange
            var userGuid = Guid.NewGuid();
            var userInfo = new UserDto { Guid = userGuid };
            _authServiceMock.Setup(x => x.LoginAsync(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync((string uname, string password) => new AuthenticationResult { Success = false });


            // Act
            var result = await _authenticationController.
                Login(new UserLoginRequest { Username = "Benny", Password = "Pass" });

            // Assert

            result.Should().BeOfType<BadRequestObjectResult>();
        }

        // public async Task<ActionResult> Refresh([FromBody] RefreshTokenRequest request)
        [Fact]
        public async Task Refresh_ShouldReturnOkResult_WhenRefreshRequestIsValid()
        {

            // Arrange
            var userGuid = Guid.NewGuid();
            string token = "exampleToken";
            string refreshToken = "exampleRefToken";
            _authServiceMock.Setup(x => x.RefreshJwtToken(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync((string token, string refreshToken) => new AuthenticationResult { Success = true, UserGuid = userGuid, Token = token, RefreshToken = refreshToken});

            var expectedResult = new SuccessAuthResponse()
            {
                UserGuid = userGuid,
                Token = token,
                RefreshToken = refreshToken
            };


            // Act
            var result = await _authenticationController.
                Refresh(new RefreshTokenRequest { Token = token, RefreshToken = refreshToken});

            // Assert

            result.Should().BeOfType<OkObjectResult>();
            var resObj = (OkObjectResult)result;
            resObj.Value.Should().BeEquivalentTo(expectedResult);
        }



        [Fact]
        public async Task Refresh_ShouldReturnBadRequest_WhenRefreshRequestIsNOTValid()
        {

            // Arrange
            var userGuid = Guid.NewGuid();
            var userInfo = new UserDto { Guid = userGuid };
            _authServiceMock.Setup(x => x.RefreshJwtToken(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync((string token, string refreshToken) => new AuthenticationResult { Success = false });


            // Act
            var result = await _authenticationController.Refresh(
                new RefreshTokenRequest { Token = "exampleToken", RefreshToken = "exampleRefToken" });

            // Assert

            result.Should().BeOfType<BadRequestObjectResult>();
        }
    }
}
