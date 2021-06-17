using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using BoomaEcommerce.Core;
using BoomaEcommerce.Domain;
using Microsoft.EntityFrameworkCore;

namespace BoomaEcommerce.Data.EfCore.Repositories
{
    public class ProductsRepository : EfCoreRepository<Product>, IProductsRepository
    {
        public ProductsRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }

        public override async Task<IEnumerable<Product>> FindAllAsync()
        {
            return await DbContext.Set<Product>().Include(p => p.Store).ToListAsync();
        }

        public override async Task<IEnumerable<Product>> FilterByAsync(Expression<Func<Product, bool>> predicateExp)
        {
            return await DbContext.Set<Product>().Include(p => p.Store).Where(predicateExp).ToListAsync();
        }
    }
}
