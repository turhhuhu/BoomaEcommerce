using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using BoomaEcommerce.Data.EfCore.Repositories;
using BoomaEcommerce.Domain.Discounts;
using BoomaEcommerce.Domain.Policies;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace BoomaEcommerce.Data.EfCore
{
    public static class DbSetExtensions
    {
        public static List<PropertyInfo> GetDbSetProperties(this DbContext context)
        {
            var dbSetProperties = new List<PropertyInfo>();
            var properties = context.GetType().GetProperties();

            foreach (var property in properties)
            {
                var setType = property.PropertyType;

                var isDbSet = setType.IsGenericType && typeof (DbSet<>).IsAssignableFrom(setType.GetGenericTypeDefinition());

                if (isDbSet)
                {
                    dbSetProperties.Add(property);
                }
            }

            return dbSetProperties;

        }
        public static IEnumerable<string> GetIncludePaths(this DbContext context, Type clrEntityType, int maxDepth = 20)
        {
            if (maxDepth < 0) throw new ArgumentOutOfRangeException(nameof(maxDepth));
            var entityType = context.Model.FindEntityType(clrEntityType);
            var includedNavigations = new HashSet<INavigation>();
            var stack = new Stack<IEnumerator<INavigation>>();
            while (true)
            {
                var entityNavigations = new List<INavigation>();
                if (stack.Count <= maxDepth)
                {
                    foreach (var navigation in entityType.GetNavigations())
                    {
                        if (includedNavigations.Add(navigation))
                            entityNavigations.Add(navigation);
                    }
                }
                if (entityNavigations.Count == 0)
                {
                    if (stack.Count > 0)
                        yield return string.Join(".", stack.Reverse().Select(e => e.Current.Name));
                }
                else
                {
                    foreach (var inverseNavigation in 
                        entityNavigations
                        .Select(navigation => navigation.Inverse)
                        .Where(inverseNavigation => inverseNavigation != null))
                    {
                        includedNavigations.Add(inverseNavigation);
                    }

                    stack.Push(entityNavigations.GetEnumerator());
                }
                while (stack.Count > 0 && !stack.Peek().MoveNext())
                    stack.Pop();

                if (!stack.TryPeek(out var navigations) || navigations.Current == null)
                {
                    break;
                }

                entityType = navigations.Current.TargetEntityType;
            }
        }
        public static IQueryable<T> Include<T>(this IQueryable<T> source, IEnumerable<string> navigationPropertyPaths)
            where T : class
        {
            return navigationPropertyPaths.Aggregate(source, (query, path) => query.Include(path));
        }
        public static async Task<Discount> GetDiscountRecursively(this ApplicationDbContext dbContext, Guid guid)
        {

            await dbContext.Discounts
                .Where(d => d.Guid == guid)
                .Select(d => d.Policy)
                .SingleOrDefaultAsync();

            var discount = await dbContext.Discounts
                .Include(d => (d as CompositeDiscount).Discounts)
                .Include(d => (d as CompositeDiscount).Operator)
                .Include(d => (d as ProductDiscount).Product)
                .FirstOrDefaultAsync(d => d.Guid == guid);


            if (discount == null)
            {
                return null;
            }

            await dbContext.Policies.GetRecursively(discount.Policy.Guid);

            if (discount is CompositeDiscount compositeDiscount && compositeDiscount.Discounts.Any())
            {
                foreach (var childDisc in compositeDiscount.Discounts)
                {
                    await dbContext.GetDiscountRecursively(childDisc.Guid);
                }
            }

            return discount;
        }

        public static async Task<Policy> GetRecursively(this IQueryable<Policy> query, Guid guid)
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
                    await query.GetRecursively(multiPolicy.Guid);
                }
            }

            if (policy is BinaryPolicy binaryPolicy)
            {
                if (binaryPolicy.FirstPolicy != null)
                {
                    await query.GetRecursively(binaryPolicy.FirstPolicy.Guid);
                }

                if (binaryPolicy.SecondPolicy != null)
                {
                    await query.GetRecursively(binaryPolicy.SecondPolicy.Guid);
                }
            }

            return policy;
        }
    }
}
