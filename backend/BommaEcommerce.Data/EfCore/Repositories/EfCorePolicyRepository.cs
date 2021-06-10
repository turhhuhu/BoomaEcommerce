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
using BoomaEcommerce.Domain.Policies.Operators;
using Microsoft.EntityFrameworkCore.Query;

namespace BoomaEcommerce.Data.EfCore.Repositories
{
    public class EfCorePolicyRepository : EfCoreRepository<Policy, ApplicationDbContext>
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
            return GetRecursively(guid, DbContext.Policies);
        }

        public static async Task<Policy> GetRecursively(Guid guid, IQueryable<Policy> query)
        {
            var policy = await query
                .Include(p => (p as MultiPolicy).Operator)
                .Include(p => (p as CompositePolicy).SubPolicies)
                .Include(p => (p as BinaryPolicy).FirstPolicy)
                .Include(p => (p as BinaryPolicy).SecondPolicy)
                .Include(p => (p as ProductPolicy).Product)
                .FirstOrDefaultAsync(p => p.Guid == guid);

            if (policy == null)
            {
                return null;
            }

            if (policy is CompositePolicy compositePolicy)
            {
                foreach (var multiPolicy in compositePolicy.SubPolicies)
                {
                    await GetRecursively(multiPolicy.Guid, query);
                }
            }

            if (policy is BinaryPolicy binaryPolicy)
            {
                if (binaryPolicy.FirstPolicy != null)
                {
                    await GetRecursively(binaryPolicy.FirstPolicy.Guid, query);
                }

                if (binaryPolicy.SecondPolicy != null)
                {
                    await GetRecursively(binaryPolicy.SecondPolicy.Guid, query);
                }
            }

            return policy;
        }

        public override async Task DeleteByIdAsync(Guid guid)
        {
            //var store = await DbContext
            //    .Set<Store>()
            //    .FirstOrDefaultAsync(s => s.StorePolicy.Guid == guid);

            //store.StorePolicy = Policy.EmptyDisc;

            await DeleteRecursively(guid, DbContext.Set<Policy>());
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
