using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BoomaEcommerce.Domain;
using Microsoft.EntityFrameworkCore;

namespace BoomaEcommerce.Data.EfCore.Repositories
{
    class EfCorePurchaseRepository : EfCoreRepository<Purchase>
    {
        public EfCorePurchaseRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }

        public override Task InsertOneAsync(Purchase entity)
        {
            //DbContext.StorePurchases.AddRange(entity.StorePurchases);
            //entity.StorePurchases.ForEach(s => DbContext.Entry(s).State = EntityState.Added);
            //entity.StorePurchases.SelectMany(s => s.PurchaseProducts).ToList().ForEach(p => DbContext.Entry(p).State = EntityState.Added);
            return base.InsertOneAsync(entity);
        }
    }
}
