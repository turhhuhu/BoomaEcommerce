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
    public class DeleteShoppingBasketUseCaseAction : UseCaseAction
    {
        [JsonRequired]
        public string ShoppingBasketLabel { get; set; }
       
        
        
        
        
        public DeleteShoppingBasketUseCaseAction(IUseCaseAction next, IServiceProvider sp, IHttpContextAccessor accessor) :
            base(next, sp, accessor)
        {
        }

        public DeleteShoppingBasketUseCaseAction(IServiceProvider sp, IHttpContextAccessor accessor) : base(sp, accessor)
        {

        }

        public DeleteShoppingBasketUseCaseAction()
        {
        }
        public override async Task NextAction(Dictionary<string,object> dict = null, ClaimsPrincipal claims = null)
        {
            if (dict is null)
            {
                throw new ArgumentException(nameof(dict));
            }

            var ShoppingBasketObj = dict[ShoppingBasketLabel];
            if (ShoppingBasketObj is not ShoppingBasketDto shoppingBasket)
            {
                throw new ArgumentException(nameof(ShoppingBasketObj));
            }


            using var scope = Sp.CreateScope();

            var userService = scope.ServiceProvider.GetRequiredService<IUsersService>();

            await userService.DeleteShoppingBasketAsync(shoppingBasket.Guid);
            
            scope.Dispose();
            await Next(dict,claims);
        }
    }
}