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
    public class DeletePurchaseProductFromShoppingBasketUseCaseAction : UseCaseAction
    {
        [JsonRequired]
        public string ShoppingBasketLabel { get; set; }
       
        [JsonRequired]
        public string PurchaseProductLabel { get; set; }
        
        
        
        public DeletePurchaseProductFromShoppingBasketUseCaseAction(IUseCaseAction next, IServiceProvider sp, IHttpContextAccessor accessor) :
            base(next, sp, accessor)
        {
        }

        public DeletePurchaseProductFromShoppingBasketUseCaseAction(IServiceProvider sp, IHttpContextAccessor accessor) : base(sp, accessor)
        {

        }

        public DeletePurchaseProductFromShoppingBasketUseCaseAction()
        {
        }
        public override async Task NextAction(Dictionary<string,object> dict = null, ClaimsPrincipal claims = null)
        {
            if (dict is null)
            {
                throw new ArgumentException(nameof(dict));
            }

            var PurchaseProductObj = dict[PurchaseProductLabel];
            if (PurchaseProductObj is not PurchaseProductDto purchaseProduct)
            {
                throw new ArgumentException(nameof(PurchaseProductObj));
            }
            
            var ShoppingBasketObj = dict[ShoppingBasketLabel];
            if (ShoppingBasketObj is not ShoppingBasketDto shoppingBasket)
            {
                throw new ArgumentException(nameof(ShoppingBasketObj));
            }


            using var scope = Sp.CreateScope();

            var userService = scope.ServiceProvider.GetRequiredService<IUsersService>();

            await userService.DeletePurchaseProductFromShoppingBasketAsync(shoppingBasket.Guid,purchaseProduct.Guid);
            
            scope.Dispose();
            await Next(dict,claims);
        }
    }
}