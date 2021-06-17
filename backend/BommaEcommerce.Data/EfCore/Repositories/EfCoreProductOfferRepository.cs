using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using BoomaEcommerce.Domain.ProductOffer;
using Microsoft.EntityFrameworkCore;

namespace BoomaEcommerce.Data.EfCore.Repositories
{
    public class EfCoreProductOfferRepository : EfCoreRepository<ProductOffer>
    {
        public EfCoreProductOfferRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }
        public override async Task<IEnumerable<ProductOffer>> FilterByAsync(Expression<Func<ProductOffer, bool>> predicateExp)
        {
            return await DbContext.ProductOffers
                .Include(o => o.ApprovedOwners)
                .Include(o => o.Product)
                .ThenInclude(p => p.Store)
                .Include(o => o.User)
                .Where(predicateExp)
                .ToListAsync();
        }

        public override Task<ProductOffer> FindByIdAsync(Guid guid)
        {
            return DbContext.ProductOffers
                .Include(o => o.ApprovedOwners)
                .Include(o => o.Product)
                .ThenInclude(p => p.Store)
                .Include(o => o.User)
                .Where(o => o.Guid == guid)
                .SingleOrDefaultAsync();
        }
    }
}
