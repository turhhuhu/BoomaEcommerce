using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoFixture;
using BommaEcommerce.Services.Tests;
using BoomaEcommerce.Domain;
using BoomaEcommerce.Services.Authentication;
using BoomaEcommerce.Services.Settings;
using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace BoomaEcommerce.Services.Tests
{
    public class AuthenticationServiceTests
    {

        private readonly IFixture _fixture;
        private const string Secret = "aaaaaaaaaaaaaaaaaaa";
        public AuthenticationServiceTests()
        {
            _fixture = new Fixture();
        }


        [Fact]
        public async Task RegisterAsync_ReturnsSuccessfulResponse_WhenUserDoesNotExists()
        {
            var userStore = new List<User>();

            var mockUserManager = DalMockFactory.MockUserManager(userStore);
            var loggerMock = new Mock<ILogger<AuthenticationService>>();

            var authService =
                new AuthenticationService(loggerMock.Object, mockUserManager.Object,
                    new JwtSettings { Secret = Secret, TokenExpirationHours = 1 });

            var tokenHandler = new JwtSecurityTokenHandler();

            // Act
            var response = await authService.RegisterAsync("user", "pass");

            // Assert
            response.Success.Should().BeTrue();
            var token = tokenHandler.ReadJwtToken(response.Token);
            ValidateTokenWithUser(token, userStore.First(usr => usr.UserName == "user"));
        }

        [Fact]
        public async Task RegisterAsync_ReturnsFailureResponse_WhenUserExists()
        {
            var userStore = new List<User>();

            var mockUserManager = DalMockFactory.MockUserManager(userStore);
            var loggerMock = new Mock<ILogger<AuthenticationService>>();

            var authService =
                new AuthenticationService(loggerMock.Object, mockUserManager.Object,
                    new JwtSettings { Secret = Secret, TokenExpirationHours = 1 });

            mockUserManager.Setup(x => x.CreateAsync(It.IsAny<User>(), It.IsAny<string>()))
                .ReturnsAsync(IdentityResult.Failed());
            // Act
            var response = await authService.RegisterAsync("user", "pass");

            // Assert
            response.Success.Should().BeFalse();
        }

        [Fact]
        public async Task LoginAsync_ReturnsSuccessfulResponse_WhenUserExists()
        {
            // Arrange
            var user = _fixture.Build<User>().Create();
            var userStore = new List<User>{user};
            var mockUserManager = DalMockFactory.MockUserManager(userStore);
            var loggerMock = new Mock<ILogger<AuthenticationService>>();

            var authService =
                new AuthenticationService(loggerMock.Object, mockUserManager.Object,
                    new JwtSettings { Secret = Secret, TokenExpirationHours = 1 });

            var tokenHandler = new JwtSecurityTokenHandler();

            // Act
            var response = await authService.LoginAsync(user.UserName, "pass");


            // Assert
            response.Success.Should().BeTrue();
            var token = tokenHandler.ReadJwtToken(response.Token);
            ValidateTokenWithUser(token, user);
        }

        [Fact]
        public async Task LoginAsync_ReturnsFailureResponse_WhenUserDoesNotExists()
        {
            // Arrange
            var userStore = new List<User>();

            var mockUserManager = DalMockFactory.MockUserManager(userStore);
            var loggerMock = new Mock<ILogger<AuthenticationService>>();

            var authService =
                new AuthenticationService(loggerMock.Object, mockUserManager.Object,
                    new JwtSettings { Secret = Secret, TokenExpirationHours = 1 });

            var tokenHandler = new JwtSecurityTokenHandler();

            // Act
            var response = await authService.LoginAsync("user", "pass");
            var foundUser = userStore.FirstOrDefault(usr => usr.UserName == "user");


            // Assert
            response.Success.Should().BeFalse();
            foundUser.Should().BeNull();
        }

        [Fact]
        public async Task LoginAsync_ReturnsFailureResponse_WhenPasswordIncorrect()
        {
            // Arrange
            var user = _fixture.Build<User>().Create();
            var userStore = new List<User> { user };

            var mockUserManager = DalMockFactory.MockUserManager(userStore);

            mockUserManager.Setup(userManager => userManager.CheckPasswordAsync(It.IsAny<User>(), It.IsAny<string>()))
                .ReturnsAsync(false);

            var loggerMock = new Mock<ILogger<AuthenticationService>>();

            var authService =
                new AuthenticationService(loggerMock.Object, mockUserManager.Object,
                    new JwtSettings { Secret = Secret, TokenExpirationHours = 1 });

            // Act
            var response = await authService.LoginAsync("user", "wrong-pass");

            // Assert
            response.Success.Should().BeFalse();
        }
        private static void ValidateTokenWithUser(JwtSecurityToken token, User user)
        {
            var sub = token.Claims.FirstOrDefault(claim => claim.Type == JwtRegisteredClaimNames.Sub);
            var jti = token.Claims.FirstOrDefault(claim => claim.Type == JwtRegisteredClaimNames.Jti);
            var uniqueName = token.Claims.FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.UniqueName);
            var guid = token.Claims.FirstOrDefault(claim => claim.Type == "guid");

            jti.Should().NotBeEquivalentTo(default(Guid));
            sub.Should().NotBeNull().And.Match<Claim>(claim => claim.Value.Equals(user.UserName));
            uniqueName.Should().NotBeNull().And.Match<Claim>(claim => claim.Value.Equals(user.UserName));
            guid.Should().NotBeNull().And.Match<Claim>(claim => claim.Value.Equals(user.Guid.ToString()));
        }

    }
}