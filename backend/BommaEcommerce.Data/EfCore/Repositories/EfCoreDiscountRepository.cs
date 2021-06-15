using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BoomaEcommerce.Core;
using BoomaEcommerce.Domain;
using BoomaEcommerce.Domain.Discounts;
using BoomaEcommerce.Domain.Policies;
using Microsoft.EntityFrameworkCore;

namespace BoomaEcommerce.Data.EfCore.Repositories
{
    public class EfCoreDiscountRepository : EfCoreRepository<Discount>
    {
        public EfCoreDiscountRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }

        public override async Task<Discount> FindByIdAsync(Guid guid)
        {
            return await DbContext.GetDiscountRecursively(guid);
        }

        public override async Task InsertOneAsync(Discount entity)
        {
            if (entity is ProductDiscount productDiscount)
            {
                productDiscount.Product = await DbContext
                    .Set<Product>()
                    .SingleOrDefaultAsync(p => p.Guid == productDiscount.Product.Guid);
                if (productDiscount.Product == null)
                {
                    return;
                }
            }
            await base.InsertOneAsync(entity);
        }

        public override Task DeleteByIdAsync(Guid guid)
        {
            return DeleteRecursively(guid, DbContext.Discounts, DbContext.Policies);
        }

        public async Task DeleteRecursively(Guid guid, DbSet<Discount> discountDbSet, DbSet<Policy> policyDbSet)
        {
            await FilterByAsync(d => d.Guid == guid, d => d.Policy);

            var discount = await discountDbSet
                .Include(d => (d as CompositeDiscount).Discounts)
                .FirstOrDefaultAsync(d => d.Guid == guid);

            if (discount == null)
            {
                return;
            }

            if (discount is CompositeDiscount compositeDiscount)
            {
                var (compositeDiscounts, simpleDiscounts) = compositeDiscount.Discounts
                    .Split(p => p is CompositeDiscount)
                    .ToList();

                foreach (var childDiscount in compositeDiscounts)
                {
                    await DeleteRecursively(childDiscount.Guid, discountDbSet, policyDbSet);
                }

                foreach (var simpleDiscount in simpleDiscounts)
                {
                    await EfCorePolicyRepository.DeleteRecursively(simpleDiscount.Policy.Guid, policyDbSet);
                }

                discountDbSet.RemoveRange(simpleDiscounts);
            }

            await EfCorePolicyRepository.DeleteRecursively(discount.Policy.Guid, policyDbSet);
            discountDbSet.Remove(discount);
        }
    }
}
