using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoFixture;
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

        private List<User> _userStore;
        private Mock<UserManager<User>> _mockUserManager;
        private Mock<ILogger<AuthenticationService>> _loggerMock;
        private AuthenticationService _authService;

        public AuthenticationServiceTests()
        {
            _fixture = new Fixture();
            _userStore = new List<User>();
            _mockUserManager = DalMockFactory.MockUserManager(_userStore);
            _loggerMock = new Mock<ILogger<AuthenticationService>>();
            _authService =
                new AuthenticationService(_loggerMock.Object, _mockUserManager.Object,
                    new JwtSettings { Secret = Secret, TokenExpirationHours = 1 });
        }

        [Fact]
        public async Task RegisterAsync_ReturnsSuccessfulResponse_WhenUserDoesNotExists()
        {
            // Arrange
            var tokenHandler = new JwtSecurityTokenHandler();

            // Act
            var response = await _authService.RegisterAsync("user", "pass");

            // Assert
            response.Success.Should().BeTrue();
            var token = tokenHandler.ReadJwtToken(response.Token);
            ValidateTokenWithUser(token, _userStore.First(usr => usr.UserName == "user"));
        }

        [Fact]
        public async Task RegisterAsync_ReturnsFailureResponse_WhenUserExists()
        {

            _mockUserManager.Setup(x => x.CreateAsync(It.IsAny<User>(), It.IsAny<string>()))
                .ReturnsAsync(IdentityResult.Failed());
            // Act
            var response = await _authService.RegisterAsync("user", "pass");

            // Assert
            response.Success.Should().BeFalse();
        }

        [Fact]
        public async Task LoginAsync_ReturnsSuccessfulResponse_WhenUserExists()
        {
            // Arrange
            var user = _fixture.Build<User>().Create();
            _userStore.Add(user);
            var tokenHandler = new JwtSecurityTokenHandler();

            // Act
            var response = await _authService.LoginAsync(user.UserName, "pass");

            // Assert
            response.Success.Should().BeTrue();
            var token = tokenHandler.ReadJwtToken(response.Token);
            ValidateTokenWithUser(token, user);
        }

        [Fact]
        public async Task LoginAsync_ReturnsFailureResponse_WhenUserDoesNotExists()
        {

            // Act
            var response = await _authService.LoginAsync("user", "pass");

            // Assert
            response.Success.Should().BeFalse();
            response.Token.Should().BeNull();
        }

        [Fact]
        public async Task LoginAsync_ReturnsFailureResponse_WhenPasswordIncorrect()
        {

            _mockUserManager.Setup(userManager => userManager.CheckPasswordAsync(It.IsAny<User>(), It.IsAny<string>()))
                .ReturnsAsync(false);

            // Act
            var response = await _authService.LoginAsync("user", "wrong-pass");

            // Assert
            response.Success.Should().BeFalse();
            response.Token.Should().BeNull();
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