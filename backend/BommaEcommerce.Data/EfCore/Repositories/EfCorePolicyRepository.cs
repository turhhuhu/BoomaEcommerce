using BoomaEcommerce.Domain.Policies;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using BoomaEcommerce.Core;
using BoomaEcommerce.Domain;
using BoomaEcommerce.Domain.Discounts;
using BoomaEcommerce.Domain.Policies.Operators;
using Microsoft.EntityFrameworkCore.Query;

namespace BoomaEcommerce.Data.EfCore.Repositories
{
    public class EfCorePolicyRepository : EfCoreRepository<Policy>
    {
        public EfCorePolicyRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }

        public override Task<TType> FindByIdAsync<TType>(Guid guid)
        {
            return DbContext.Set<Policy>()
                .Include(p => (p as MultiPolicy).Operator)
                .Include(p => (p as CompositePolicy).SubPolicies)
                .Include(p => (p as ProductPolicy).Product)
                .OfType<TType>().FirstOrDefaultAsync(p => p.Guid == guid);
        }

        public override Task InsertOneAsync(Policy policy)
        {
            if (policy is ProductPolicy productPolicy)
            {
                DbContext.Products.Attach(productPolicy.Product);
            }
            return base.InsertOneAsync(policy);
        }

        public override Task<Policy> FindByIdAsync(Guid guid)
        {
            return DbContext.Policies.GetRecursively(guid);
        }

        public override async Task DeleteByIdAsync(Guid guid)
        {
            await DeleteRecursively(guid, DbContext.Set<Policy>());
            var store = await DbContext
                .Set<Store>()
                .FirstOrDefaultAsync(s => s.StorePolicy.Guid == guid);

            var discount = await DbContext
                .Set<Discount>()
                .FirstOrDefaultAsync(s => s.Policy.Guid == guid);

            if (discount != null)
            {
                discount.Policy = Policy.Empty;
                await InsertOneAsync(discount.Policy);
            }

            if (store != null)
            {
                store.StorePolicy = Policy.Empty;
                await InsertOneAsync(store.StorePolicy);
            }
        }
        public static async Task DeleteRecursively(Guid guid, DbSet<Policy> dbSet)
        {
            var policy = await dbSet
                .Include(p => (p as CompositePolicy).SubPolicies)
                .Include(p => (p as BinaryPolicy).FirstPolicy)
                .Include(p => (p as BinaryPolicy).SecondPolicy)
                .FirstOrDefaultAsync(p => p.Guid == guid);

            if (policy == null)
            {
                return;
            }

            if (policy is CompositePolicy compositePolicy && compositePolicy.SubPolicies.Any()){

                var (multiPolicies, simplePolicies) = compositePolicy.SubPolicies.Split(p => p is MultiPolicy);
                foreach (var multiPolicy in multiPolicies)
                {
                    await DeleteRecursively(multiPolicy.Guid, dbSet);
                }
                dbSet.RemoveRange(simplePolicies);
            }
            else if (policy is BinaryPolicy binaryPolicy)
            {
                if (binaryPolicy.FirstPolicy != null)
                {
                    await DeleteRecursively(binaryPolicy.FirstPolicy.Guid, dbSet);
                }

                if (binaryPolicy.SecondPolicy != null)
                {
                    await DeleteRecursively(binaryPolicy.SecondPolicy.Guid, dbSet);
                }
            }
            dbSet.Remove(policy);
        }
    }
}
