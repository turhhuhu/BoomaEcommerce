using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoFixture;
using BoomaEcommerce.Data;
using BoomaEcommerce.Domain;
using BoomaEcommerce.Services.Authentication;
using BoomaEcommerce.Services.DTO;
using BoomaEcommerce.Services.Settings;
using BoomaEcommerce.Tests.CoreLib;
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
        private Dictionary<Guid, RefreshToken> _refreshTokens;
        private Mock<IRepository<RefreshToken>> _refreshTokenRepo;
        public AuthenticationServiceTests()
        {
            _fixture = new Fixture();
            _userStore = new List<User>();
            _mockUserManager = DalMockFactory.MockUserManager(_userStore);
            _loggerMock = new Mock<ILogger<AuthenticationService>>();
            _refreshTokens = new Dictionary<Guid, RefreshToken>();
            _refreshTokenRepo = DalMockFactory.MockRepository(_refreshTokens);
            _authService =
                new AuthenticationService(_loggerMock.Object, _mockUserManager.Object,
                    new JwtSettings
                    {
                        Secret = Secret, TokenLifeTime = TimeSpan.FromHours(1), RefreshTokenExpirationMonthsAmount = 6

                    }, null, _refreshTokenRepo.Object, MapperFactory.GetMapper());

        }

        [Fact]
        public async Task RegisterAsync_ReturnsSuccessfulResponse_WhenUserDoesNotExists()
        {
            // Arrange
            var tokenHandler = new JwtSecurityTokenHandler();

            // Act
            var response = await _authService.RegisterAsync(new UserDto {UserName = "user"}, "pass");

            // Assert
            response.Success.Should().BeTrue();
            var refreshToken = _refreshTokens.Values.FirstOrDefault(rfToken => rfToken.Token == response.RefreshToken);
            refreshToken.Should().NotBeNull();
            var token = tokenHandler.ReadJwtToken(response.Token);
            ValidateTokenWithUser(token, _userStore.First(usr => usr.UserName == "user"));
        }

        [Fact]
        public async Task RegisterAsync_ReturnsFailureResponse_WhenUserExists()
        {

            _mockUserManager.Setup(x => x.CreateAsync(It.IsAny<User>(), It.IsAny<string>()))
                .ReturnsAsync(IdentityResult.Failed());
            // Act
            var response = await _authService.RegisterAsync(new UserDto { UserName = "user" }, "pass");

            // Assert
            response.Success.Should().BeFalse();
        }
        [Fact]
        public async Task RegisterAdminAsync_ReturnsSuccessResponse_WhenUserDoNotExists()
        {

            // Arrange
            var tokenHandler = new JwtSecurityTokenHandler();

            // Act
            var response = await _authService.RegisterAdminAsync(new AdminUserDto { UserName = "user" }, "pass");

            // Assert
            response.Success.Should().BeTrue();
            var refreshToken = _refreshTokens.Values.FirstOrDefault(rfToken => rfToken.Token == response.RefreshToken);
            refreshToken.Should().NotBeNull();
            var token = tokenHandler.ReadJwtToken(response.Token);
            ValidateTokenWithUser(token, _userStore.First(usr => usr.UserName == "user"));
            var adminRoleClaim = token.Claims.FirstOrDefault(claim => claim.Value == UserRoles.AdminRole );
            adminRoleClaim.Should().NotBeNull();
        }

        [Fact]
        public async Task LoginAsync_ReturnsSuccessfulResponse_WhenUserExists()
        {
            // Arrange
            var user = _fixture.Build<User>().Create();
            var tokenHandler = new JwtSecurityTokenHandler();
            var registerResponse = await _authService.RegisterAsync(new UserDto { UserName = user.UserName }, "pass");
            user.Guid = registerResponse.UserGuid;

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
            AuthenticationResult result = await _authService.LoginAsync("user", "pass");

            // Assert
            result.Success.Should().BeFalse();
            result.Token.Should().BeNull();
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