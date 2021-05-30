using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BoomaEcommerce.Domain;

namespace BoomaEcommerce.Data.EfCore.Repositories
{
    public interface IProductsRepository : IRepository<Product>
    {
    }
}
