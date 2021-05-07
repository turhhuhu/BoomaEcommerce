using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BoomaEcommerce.Domain;
using Microsoft.AspNetCore.Identity;

namespace BoomaEcommerce.Data
{
    public interface IPurchaseUnitOfWork
    {
        IRepository<Purchase> PurchaseRepository { get; set; }
        UserManager<User> UserRepository { get; set; }
        IRepository<Product> ProductRepository { get; set; }
        IRepository<ShoppingCart> ShoppingCartRepository { get; set; }
        IRepository<StoreOwnership> StoreOwnershipRepository { get; set; }
        IRepository<Notification> NotificationRepository { get; set; }
        Task SaveAsync();
    }
}
