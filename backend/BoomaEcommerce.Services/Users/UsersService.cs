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
        private readonly IUserUnitOfWork _userUnitOfWork;


        public UsersService(IMapper mapper, ILogger<UsersService> logger,
             IUserUnitOfWork userUnitOfWork)
        {
            _mapper = mapper;
            _logger = logger;
            _userUnitOfWork = userUnitOfWork;
        }

        public async Task<ShoppingCartDto> GetShoppingCartAsync(Guid userGuid)
        {
            try
            {
                var shoppingCart = await _userUnitOfWork.ShoppingCartRepo
                    .FindOneAsync(x => x.User.Guid == userGuid);
                
                if (shoppingCart is not null) return _mapper.Map<ShoppingCartDto>(shoppingCart);
                
                shoppingCart = new ShoppingCart {User = new User {Guid = userGuid}};
                await _userUnitOfWork.ShoppingCartRepo.InsertOneAsync(shoppingCart);
                return _mapper.Map<ShoppingCartDto>(shoppingCart);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return null;
            }
        }

        public async Task<bool> CreateShoppingBasketAsync(Guid shoppingCartGuid, ShoppingBasketDto shoppingBasketDto)
        {
            try
            {
                var shoppingBasket = _mapper.Map<ShoppingBasket>(shoppingBasketDto);
                var shoppingCart = await _userUnitOfWork.ShoppingCartRepo
                    .FindOneAsync(x => x.Guid == shoppingCartGuid);
                if (!shoppingCart.AddShoppingBasket(shoppingBasket))
                {
                    return false;
                }
                await _userUnitOfWork.ShoppingBasketRepo.InsertOneAsync(shoppingBasket);
                await _userUnitOfWork.ShoppingCartRepo.ReplaceOneAsync(shoppingCart);
                await _userUnitOfWork.SaveAsync();
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return false;
            }
        }
        
        public async Task<bool> AddPurchaseProductToShoppingBasketAsync(Guid shoppingBasketGuid,
            PurchaseProductDto purchaseProductDto)
        {
            try
            {
                var purchaseProduct = _mapper.Map<PurchaseProduct>(purchaseProductDto);
                var shoppingBasket = await _userUnitOfWork.ShoppingBasketRepo
                    .FindOneAsync(x => x.Guid == shoppingBasketGuid);
                if (!shoppingBasket.AddPurchaseProduct(purchaseProduct))
                {
                    return false;
                }

                await _userUnitOfWork.PurchaseProductRepo.InsertOneAsync(purchaseProduct);
                await _userUnitOfWork.ShoppingBasketRepo.ReplaceOneAsync(shoppingBasket);
                await _userUnitOfWork.SaveAsync();
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return false;
            }
        }

        public async Task<bool> DeleteProductFromShoppingBasketAsync(Guid shoppingBasketGuid, Guid purchaseProductGuid)
        {
            try
            {
                var shoppingBasket = await _userUnitOfWork.ShoppingBasketRepo
                    .FindOneAsync(x => x.Guid == shoppingBasketGuid);
                if (!shoppingBasket.RemovePurchaseProduct(purchaseProductGuid))
                {
                    return false;
                }
                await _userUnitOfWork.ShoppingBasketRepo.InsertOneAsync(shoppingBasket);
                await _userUnitOfWork.SaveAsync();
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return false;
            }
        }

        public async Task DeleteShoppingBasketAsync(Guid shoppingBasketGuid)
        {
            try
            {
                await _userUnitOfWork.ShoppingBasketRepo.DeleteOneAsync(x => x.Guid == shoppingBasketGuid);
                await _userUnitOfWork.SaveAsync();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
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
