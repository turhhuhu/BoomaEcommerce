﻿using System;
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
    public class CreateShoppingBasketUseCaseAction : UseCaseAction
    {
        [JsonRequired]
        public string ShoppingCartLabel { get; set; }
        [JsonRequired]
        public ShoppingBasketDto NewShoppingBasket { get; set; }
        
        
        
        public CreateShoppingBasketUseCaseAction(IUseCaseAction next, IServiceProvider sp, IHttpContextAccessor accessor) :
            base(next, sp, accessor)
        {
        }

        public CreateShoppingBasketUseCaseAction(IServiceProvider sp, IHttpContextAccessor accessor) : base(sp, accessor)
        {

        }

        public CreateShoppingBasketUseCaseAction()
        {
        }
        public override async Task NextAction(Dictionary<string,object> dict = null, ClaimsPrincipal claims = null)
        {
            if (dict is null)
            {
                throw new ArgumentException(nameof(dict));
            }

           
            
            var ShoppingCartObj = dict[ShoppingCartLabel];
            if (ShoppingCartObj is not ShoppingCartDto shoppingCart)
            {
                throw new ArgumentException(nameof(ShoppingCartObj));
            }


            using var scope = Sp.CreateScope();

            var userService = scope.ServiceProvider.GetRequiredService<IUsersService>();

            var shoppingBasket = await userService.CreateShoppingBasketAsync(shoppingCart.Guid,NewShoppingBasket);
            dict.Add(Label,shoppingBasket);
            scope.Dispose();
            await Next(dict,claims);
        }
    }
}