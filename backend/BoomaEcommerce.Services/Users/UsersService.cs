using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using BoomaEcommerce.Data;
using BoomaEcommerce.Domain;
using BoomaEcommerce.Services.DTO;
using BoomaEcommerce.Services.External;
using BoomaEcommerce.Services.Purchases;
using Microsoft.Extensions.Logging;
using BoomaEcommerce.Services.Stores;

namespace BoomaEcommerce.Services.Users
{
    public class UsersService : IUsersService
    {        
        private readonly IMapper _mapper;
        private readonly ILogger<UsersService> _logger;

        public UsersService(IMapper mapper, ILogger<UsersService> logger)
        {
            _mapper = mapper;
            _logger = logger;
        }

        public Task<ShoppingCartDto> GetShoppingCartAsync(Guid userGuid)
        {
            throw new NotImplementedException();
        }

        public Task CreateShoppingBasketAsync(Guid shoppingCartGuid, ShoppingBasketDto shoppingBasket)
        {
            throw new NotImplementedException();
        }
        public Task<ShoppingCartDto> GetShoppingCartAsync(string userId)
        {
            throw new NotImplementedException();
        }

        public Task AddProductToShoppingBasketAsync(Guid shoppingBasketGuid, PurchaseProductDto purchaseProduct)
        {
            throw new NotImplementedException();
        }

        public Task DeleteProductFromShoppingBasketAsync(Guid shoppingBasketGuid, Guid purchaseProductGuid)
        {
            throw new NotImplementedException();
        }

        public Task DeleteShoppingBasketAsync(Guid shoppingBasketGuid)
        {
            throw new NotImplementedException();
        }

        public Task<UserDto> GetUserInfoAsync(Guid userGuid)
        {
            throw new NotImplementedException();
        }

        public Task UpdateUserInfoAsync(UserDto user)
        {
            throw new NotImplementedException();
        }
    }
}
