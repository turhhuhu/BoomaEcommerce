﻿using System.Threading.Tasks;
using BoomaEcommerce.Domain;
using Microsoft.AspNetCore.Identity;

namespace BoomaEcommerce.Data
{
    public interface IUserUnitOfWork
    {
        IRepository<ShoppingBasket> ShoppingBasketRepo { get; set; }
        IRepository<ShoppingCart> ShoppingCartRepo { get; set; }
        IRepository<PurchaseProduct> PurchaseProductRepo { get; set; }
        UserManager<User> UserManager { get; set; }
        Task SaveAsync();
    }
}