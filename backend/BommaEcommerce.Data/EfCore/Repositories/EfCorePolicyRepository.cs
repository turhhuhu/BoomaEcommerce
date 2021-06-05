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
                .OfType<TType>().FirstOrDefaultAsync(p => p.Guid == guid);
        }

        public override Task<Policy> FindByIdAsync(Guid guid)
        {
            return GetRecursively(guid);
        }

        private async Task<Policy> GetRecursively(Guid guid)
        {
            var policy = await DbContext.Set<Policy>()
                .Include(p => (p as MultiPolicy).Operator)
                .Include(p => (p as CompositePolicy).SubPolicies)
                .Include(p => (p as BinaryPolicy).FirstPolicy)
                .Include(p => (p as BinaryPolicy).SecondPolicy)
                .FirstAsync(p => p.Guid == guid);

            if (policy is CompositePolicy compositePolicy)
            {
                foreach (var multiPolicy in compositePolicy.SubPolicies.Where(p => p is MultiPolicy))
                {
                    await GetRecursively(multiPolicy.Guid);
                }
            }

            if (policy is BinaryPolicy binaryPolicy)
            {
                if (binaryPolicy.FirstPolicy is MultiPolicy)
                {
                    await GetRecursively(binaryPolicy.FirstPolicy.Guid);
                }
                if (binaryPolicy.FirstPolicy is MultiPolicy)
                {
                    await GetRecursively(binaryPolicy.FirstPolicy.Guid);
                }
            }

            return policy;
        }

        public override async Task DeleteByIdAsync(Guid guid)
        {
            var store = await DbContext
                .Set<Store>()
                .FirstOrDefaultAsync(s => s.StorePolicy.Guid == guid);

            store.StorePolicy = Policy.Empty;

            await DeleteRecursively(guid);
        }
        private async Task DeleteRecursively(Guid guid)
        {
            var policy = await DbContext.Set<Policy>()
                .Include(p => (p as CompositePolicy).SubPolicies)
                .FirstOrDefaultAsync(p => p.Guid == guid);

            if (policy == null)
            {
                return;
            }

            if (policy is CompositePolicy compositePolicy && compositePolicy.SubPolicies.Any()){

                var (multiPolicies, simplePolicies) = compositePolicy.SubPolicies.Split(p => p is MultiPolicy);
                foreach (var multiPolicy in multiPolicies)
                {
                    await DeleteRecursively(multiPolicy.Guid);
                }
                DbContext.Set<Policy>().RemoveRange(simplePolicies);
            }
            else if (policy is BinaryPolicy binaryPolicy)
            {
                await DeleteRecursively(binaryPolicy.FirstPolicy.Guid);
                await DeleteRecursively(binaryPolicy.SecondPolicy.Guid);
            }


            DbContext.Set<Policy>().Remove(policy);
        }
    }
}
