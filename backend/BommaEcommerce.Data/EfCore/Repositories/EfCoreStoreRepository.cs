using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BoomaEcommerce.Domain;
using Microsoft.EntityFrameworkCore;

namespace BoomaEcommerce.Data.EfCore.Repositories
{
    public class EfCoreStoreRepository : EfCoreRepository<Store>
    {
        public EfCoreStoreRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }

        public override async Task InsertOneAsync(Store entity)
        {
            await DbContext.Policies.AddAsync(entity.StorePolicy);
            await DbContext.Discounts.AddAsync(entity.StoreDiscount); 
            await base.InsertOneAsync(entity);
        }

        public override async Task<Store> FindByIdAsync(Guid guid)
        {
            await DbContext.Stores
                .Where(s => s.Guid == guid)
                .Select(s => s.StoreDiscount)
                .SingleOrDefaultAsync();

            await DbContext.Stores
                .Where(s => s.Guid == guid)
                .Select(s => s.StorePolicy)
                .SingleOrDefaultAsync();

            var store = await DbContext
                .Stores
                .Include(s => s.StoreDiscount)
                .Include(s => s.StorePolicy)
                .SingleOrDefaultAsync(s => s.Guid == guid);

            if (store == null)
            {
                return null;
            }

            await DbContext.GetDiscountRecursively(store.StoreDiscount.Guid);
            await DbContext.Policies.GetRecursively(store.StorePolicy.Guid);

            return store;
        }
    }
}
