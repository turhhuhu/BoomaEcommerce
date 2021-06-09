using System;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.Internal;
using BoomaEcommerce.Core.Exceptions;
using BoomaEcommerce.Data;
using BoomaEcommerce.Domain;
using BoomaEcommerce.Services.DTO;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

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

                var user = await _userUnitOfWork.UserRepository.FindByIdAsync(userGuid);

                shoppingCart = new ShoppingCart(user);
                await _userUnitOfWork.ShoppingCartRepo.InsertOneAsync(shoppingCart);
                await _userUnitOfWork.SaveAsync();

                return _mapper.Map<ShoppingCartDto>(shoppingCart);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Failed to get shopping cart for user with UserDto {User}", userGuid);
                return null;
            }
        }

        public async Task<ShoppingBasketDto> CreateShoppingBasketAsync(Guid shoppingCartGuid, ShoppingBasketDto shoppingBasketDto)
        {
            try
            {
                var shoppingBasket = _mapper.Map<ShoppingBasket>(shoppingBasketDto);

                var shoppingCart = await _userUnitOfWork.ShoppingCartRepo
                    .FindOneAsync(x => x.Guid == shoppingCartGuid) 
                                   ?? new ShoppingCart(await _userUnitOfWork
                                       .UserRepository.
                                       FindByIdAsync(shoppingCartGuid));

                shoppingBasket.PurchaseProducts.ForAll(pp => _userUnitOfWork.AttachNoChange(pp.Product));
                _userUnitOfWork.AttachNoChange(shoppingBasket.Store);

                if (!shoppingCart.AddShoppingBasket(shoppingBasket))
                {
                    return null;
                }
                await _userUnitOfWork.ShoppingBasketRepo.InsertOneAsync(shoppingBasket);
                await _userUnitOfWork.SaveAsync();
                return _mapper.Map<ShoppingBasketDto>(shoppingBasket);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Failed to create shopping basket for shopping cart with UserDto {ShoppingCartGuid}", shoppingCartGuid);
                return null;
            }
        }
        
        public async Task<PurchaseProductDto> AddPurchaseProductToShoppingBasketAsync(Guid userGuid, Guid shoppingBasketGuid,
            PurchaseProductDto purchaseProductDto)
        {
            try
            {
                var purchaseProduct = _mapper.Map<PurchaseProduct>(purchaseProductDto);

                _userUnitOfWork.AttachNoChange(purchaseProduct.Product);

                var shoppingBasket = await _userUnitOfWork.ShoppingBasketRepo.FindByIdAsync(shoppingBasketGuid);

                if (shoppingBasket == null)
                {
                    return null;
                }

                if (!shoppingBasket.AddPurchaseProduct(purchaseProduct))
                {
                    return null;
                }
                await _userUnitOfWork.SaveAsync();
                return _mapper.Map<PurchaseProductDto>(purchaseProduct);
            }
            catch (PolicyValidationException)
            {
                _logger.LogError($"Store policy failed after adding the product {purchaseProductDto.ProductGuid}.");
                throw;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Failed to add purchase product to shopping basket with UserDto {ShoppingBasketGuid}", shoppingBasketGuid);
                return null;
            }
        }

        public async Task<bool> DeletePurchaseProductFromShoppingBasketAsync(Guid shoppingBasketGuid, Guid purchaseProductGuid)
        {
            try
            {
                var shoppingBasket = await _userUnitOfWork.ShoppingBasketRepo
                    .FindOneAsync(x => x.Guid == shoppingBasketGuid);
                if (shoppingBasket == null || !shoppingBasket.RemovePurchaseProduct(purchaseProductGuid))
                {
                    return false;
                }
                await _userUnitOfWork.ShoppingBasketRepo.ReplaceOneAsync(shoppingBasket);
                await _userUnitOfWork.SaveAsync();
                return true;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Failed to Delete purchase product from shopping basket with UserDto {ShoppingBasketGuid}", shoppingBasketGuid);
                return false;
            }
        }

        public async Task<bool> DeleteShoppingBasketAsync(Guid shoppingBasketGuid)
        {
            try
            {
                await _userUnitOfWork.ShoppingBasketRepo.DeleteOneAsync(x => x.Guid == shoppingBasketGuid);
                await _userUnitOfWork.SaveAsync();
                return true;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Failed to Delete shopping basket with UserDto {ShoppingBasketGuid}", shoppingBasketGuid);
                return false;
            }
        }

        public async Task<UserDto> GetUserInfoAsync(Guid userGuid)
        {
            try
            {
                _logger.LogInformation("Getting user info for user with guid {userGuid}", userGuid);
                var user = await _userUnitOfWork.UserRepository.FindByIdAsync(userGuid);
                return _mapper.Map<UserDto>(user);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Failed to get info for userDto {userGuid}", userGuid);
                return null;
            }
        }

        public async Task<bool> UpdateUserInfoAsync(UserDto userDto)
        {
            try
            {
                _logger.LogInformation("Updating user info for user with guid {userGuid}", userDto.Guid);
                var user = _mapper.Map<User>(userDto);
                await _userUnitOfWork.UserRepository.ReplaceOneAsync(user);
                return true;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Failed to update info for userDto {userGuid}", userDto.Guid);
                return false;
            }
        }

        public async Task<BasicUserInfoDto> GetBasicUserInfoAsync(string userName)
        {
            try
            {
                _logger.LogInformation("Getting user by username {username}.", userName);
                var users = await _userUnitOfWork.UserRepository.FilterByAsync(u => u.UserName.Equals(userName));
                var user = users.ToList().First(); 
                return _mapper.Map<BasicUserInfoDto>(user);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Failed to get user with username {userName}", userName);
                return null;
            }
        }

        public async Task<bool> SetNotificationAsSeen(Guid userGuid, Guid notificationGuid)
        {
            try
            {
                _logger.LogInformation($"Making attempt to set notification with guid {notificationGuid} to seen.");
                var user = await _userUnitOfWork.UserRepository.FindByIdAsync(userGuid);

                var notification = user?.Notifications.FirstOrDefault(n => n.Guid == notificationGuid);

                if (notification == null)
                {
                    return false;
                }

                notification.WasSeen = true;
                await _userUnitOfWork.SaveAsync();
                _logger.LogInformation($"Notification with guid {notificationGuid} was set to seen successfully.");
                return true;
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Failed to set notification with guid {notificationGuid} to see.");
                return false;
            }
        }
    }
}
