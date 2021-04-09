using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BoomaEcommerce.Domain;

namespace BoomaEcommerce.Data
{
    public interface IPurchaseUnitOfWork
    {
        IRepository<Purchase> PurchaseRepository { get; set; }
        IRepository<User> UserRepository { get; set; }
        IRepository<Product> ProductRepository { get; set; }
        void SaveAsync();
    }
}
