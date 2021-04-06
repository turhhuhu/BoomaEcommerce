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
using BoomaEcommerce.Services.DTO;
using BoomaEcommerce.Services.Settings;
using FluentAssertions;
using FluentAssertions.Execution;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace BoomaEcommerce.Services.Tests
{
    public class AuthenticationServiceTests
    {
        private readonly IFixture _fixture;
        public AuthenticationServiceTests()
        {
            _fixture = new Fixture();
        }

        [Fact]
        public async Task LoginAsync_ReturnsSuccessfulResponse_WhenUserExists()
        {
            // Arrange
            var secret = "aaaaaaaaaaaaaaaaaaa";

            var user = _fixture.Build<User>().Create();

            var mockUserManager = DalMockFactory.MockUserManager(user);
            var loggerMock = new Mock<ILogger<AuthenticationService>>();

            var authService =
                new AuthenticationService(loggerMock.Object, mockUserManager.Object,
                    new JwtSettings { Secret = secret, TokenExpirationHours = 1 });

            var tokenHandler = new JwtSecurityTokenHandler();

            // Act
            var response = await authService.LoginAsync(user.UserName, "pass");


            // Assert
            response.Success.Should().BeTrue();
            var token = tokenHandler.ReadJwtToken(response.Token);
            var sub = token.Claims.FirstOrDefault(claim => claim.Type == JwtRegisteredClaimNames.Sub);
            var jti = token.Claims.FirstOrDefault(claim => claim.Type == JwtRegisteredClaimNames.Jti);
            var uniqueName = token.Claims.FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.UniqueName);
            var guid = token.Claims.FirstOrDefault(claim => claim.Type == "guid");

            jti.Should().NotBeEquivalentTo(default(Guid));
            sub.Should().NotBeNull().And.Match<Claim>(claim => claim.Value.Equals(user.UserName));
            uniqueName.Should().NotBeNull().And.Match<Claim>(claim => claim.Value.Equals(user.UserName));
            guid.Should().NotBeNull().And.Match<Claim>(claim => claim.Value.Equals(user.Guid.ToString()));
        }

        [Fact]
        public async Task Test()
        {
            var entities = new Dictionary<Guid, Product>();
            var repoMock = DalMockFactory.MockRepository(entities);

            var repo = repoMock.Object;
            var product = new Product {Name = "kaki"};
            await repo.InsertOneAsync(product);
            await repo.InsertOneAsync(new Product { Name = "kaki" });
            var x = await repo.FilterByAsync(x => x.Name == product.Name);
            var prodFound = entities[product.Guid];
        }

    }
}