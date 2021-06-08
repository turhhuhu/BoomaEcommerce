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

    public class StoreOwnershipRepository : EfCoreRepository<StoreOwnership, ApplicationDbContext>
    {
        public StoreOwnershipRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }

        public override async Task<IEnumerable<StoreOwnership>> FindAllAsync()
        {
            return await DbContext.Set<StoreOwnership>().Include(p => p.StoreOwnerships).ToListAsync();
        }

        public override Task<StoreOwnership> FindOneAsync(Expression<Func<StoreOwnership, bool>> predicateExp)
        {
            return DbContext.Set<StoreOwnership>()
               .Include(DbContext.GetIncludePaths(typeof(StoreOwnership)))
               .OrderByDescending(x => x.Guid).AsSplitQuery().FirstOrDefaultAsync(predicateExp);
        }

        public override async Task<StoreOwnership> FindByIdAsync(Guid guid)
        {
            return await FindOneAsync((x) => x.Guid == guid);
        }


    }
}
