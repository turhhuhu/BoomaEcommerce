using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using AutoFixture;
using BoomaEcommerce.Domain;
using BoomaEcommerce.Services.Authorization.Handlers;
using BoomaEcommerce.Services.DTO;
using FluentAssertions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;
using Xunit;
namespace BoomaEcommerce.Services.Tests.HandlerTests
{
    public class UserOwnsResourceHandlerTests
    {
        private readonly IFixture _fixture;
        public UserOwnsResourceHandlerTests()
        {
            _fixture = new Fixture();
        }
        
        [Fact]
        public async Task HandleRequirementAsync_AuthorizationSuccessful_WhenUserOwnsResource()
        {
            // Arrange
            var cart = _fixture.Build<ShoppingCartDto>().Create();
            var requirements = new[] { new UserOwnsResourceRequirement() };
            var user = new ClaimsPrincipal(
                new ClaimsIdentity(
                    new Claim[] {
                        new("guid", cart.User.Guid.ToString()),
                    })
            );
            var context = new AuthorizationHandlerContext(requirements, user, cart);
            var handler = new UserOwnsResourceHandler<ShoppingCartDto>();

            //Act
            await handler.HandleAsync(context);

            //Assert
            context.HasSucceeded.Should().BeTrue();
        }
        [Fact]
        public async Task HandleRequirementAsync_AuthorizationFailure_WhenUserDoNotOwnResource()
        {
            // Arrange
            var cart = _fixture.Build<ShoppingCartDto>().Create();
            var requirements = new[] { new UserOwnsResourceRequirement() };
            var user = new ClaimsPrincipal(
                new ClaimsIdentity(
                    new Claim[] {
                        new("guid", Guid.NewGuid().ToString()),
                    })
            );
            var context = new AuthorizationHandlerContext(requirements, user, cart);
            var handler = new UserOwnsResourceHandler<ShoppingCartDto>();

            //Act
            await handler.HandleAsync(context);

            //Assert
            context.HasSucceeded.Should().BeFalse();
        }
    }
}
