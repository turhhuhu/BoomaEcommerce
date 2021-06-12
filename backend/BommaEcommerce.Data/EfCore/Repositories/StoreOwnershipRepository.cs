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
               .Include(DbContext.GetIncludePaths(typeof(StoreOwnership), 10))
               .OrderByDescending(x => x.Guid).AsSplitQuery().FirstOrDefaultAsync(predicateExp);
        }



        public async Task<StoreOwnership> GetRecursively(Guid guid)
        {
            var ownership = await DbContext.Set<StoreOwnership>()
                .Include(o => o.Store)
                .Include(o => o.User)
                .Include(o => o.StoreManagements)
                .Include(o => o.StoreOwnerships)
                .OrderByDescending(x => x.Guid)
                .AsSplitQuery()
                .FirstOrDefaultAsync(o => o.Guid == guid);

            if (ownership == null || !ownership.StoreOwnerships.Any())
            {
                return ownership;
            }

            foreach (var storeOwnership in ownership.StoreOwnerships)
            {
                await GetRecursively(storeOwnership.Guid);
            }
            return ownership;
        }

        public override async Task<StoreOwnership> FindByIdAsync(Guid guid)
        {
            return await GetRecursively(guid);
        }

        public override async Task<IEnumerable<StoreOwnership>> FilterByAsync(Expression<Func<StoreOwnership, bool>> predicateExp)
        {
            return await DbContext
                .Set<StoreOwnership>()
                .Include(o => o.Store)
                .Include(o => o.User)
                .Where(predicateExp)
                .ToListAsync();
        }
    }
}
