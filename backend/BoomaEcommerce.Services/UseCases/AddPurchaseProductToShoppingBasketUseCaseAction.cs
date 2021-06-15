using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using BoomaEcommerce.Services.DTO;
using BoomaEcommerce.Services.Users;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;

namespace BoomaEcommerce.Services.UseCases
{
    public class AddPurchaseProductToShoppingBasketUseCaseAction : UseCaseAction
    {
        [JsonRequired]
        public string ShoppingBasketLabel { get; set; }
        [JsonRequired]
        public string UserLabel { get; set; }
        [JsonRequired]
        public PurchaseProductDto NewPurchaseProduct { get; set; }
        
        
        
        public AddPurchaseProductToShoppingBasketUseCaseAction(IUseCaseAction next, IServiceProvider sp, IHttpContextAccessor accessor) :
            base(next, sp, accessor)
        {
        }

        public AddPurchaseProductToShoppingBasketUseCaseAction(IServiceProvider sp, IHttpContextAccessor accessor) : base(sp, accessor)
        {

        }

        public AddPurchaseProductToShoppingBasketUseCaseAction()
        {
        }
        public override async Task NextAction(Dictionary<string,object> dict = null, ClaimsPrincipal claims = null)
        {
            if (dict is null)
            {
                throw new ArgumentException(nameof(dict));
            }

            var UserObj = dict[UserLabel];
            if (UserObj is not UserDto user)
            {
                throw new ArgumentException(nameof(UserObj));
            }
            
            var ShoppingBasketObj = dict[ShoppingBasketLabel];
            if (ShoppingBasketObj is not ShoppingBasketDto shoppingBasket)
            {
                throw new ArgumentException(nameof(ShoppingBasketObj));
            }


            using var scope = Sp.CreateScope();

            var userService = scope.ServiceProvider.GetRequiredService<IUsersService>();

            var purchaseProduct = await userService.AddPurchaseProductToShoppingBasketAsync(user.Guid,shoppingBasket.Guid,NewPurchaseProduct);
            dict.Add(Label,purchaseProduct);
            scope.Dispose();
            await Next(dict,claims);
        }
    }
}