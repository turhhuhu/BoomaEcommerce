using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BoomaEcommerce.Core;
using BoomaEcommerce.Domain.Discounts;
using BoomaEcommerce.Domain.Policies;
using Microsoft.EntityFrameworkCore;

namespace BoomaEcommerce.Data.EfCore.Repositories
{
    public class EfCoreDiscountRepository : EfCoreRepository<Discount, ApplicationDbContext>
    {
        public EfCoreDiscountRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }

        public override async Task<Discount> FindByIdAsync(Guid guid)
        {
            return await GetRecursively(guid, DbContext.Set<Discount>(), DbContext.Set<Policy>());
        }

        public override Task DeleteByIdAsync(Guid guid)
        {
            return DeleteRecursively(guid, DbContext.Set<Discount>(), DbContext.Set<Policy>());
        }

        public static async Task DeleteRecursively(Guid guid, DbSet<Discount> discountDbSet, DbSet<Policy> policyDbSet)
        {
            var discount = await discountDbSet
                .Include(d => d.Policy)
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

        public async Task<Discount> GetRecursively(Guid guid,
            IQueryable<Discount> discountQuery,
            IQueryable<Policy> policyQuery)
        {
            var policy = await FilterByAsync(d => d.Guid == guid, d => d.Policy);

            var discount = await discountQuery
                .Include(d => (d as CompositeDiscount).Discounts)
                .Include(d => (d as ProductDiscount).Product)
                .FirstOrDefaultAsync(d => d.Guid == guid);



            if (discount == null)
            {
                return null;
            }

            await EfCorePolicyRepository.GetRecursively(discount.Policy.Guid, policyQuery);

            if (discount is CompositeDiscount compositeDiscount && compositeDiscount.Discounts.Any())
            {
                foreach (var childDisc in compositeDiscount.Discounts)
                {
                    await GetRecursively(childDisc.Guid, discountQuery, policyQuery);
                }
            }

            return discount;
        }
    }
}
