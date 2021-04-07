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
        public IRepository<Purchase> PurchaseRepository { get; set; }
        public IRepository<User> UserRepository { get; set; }
        public IRepository<Product> ProductRepository { get; set; }
        public void Save();
    }
}
