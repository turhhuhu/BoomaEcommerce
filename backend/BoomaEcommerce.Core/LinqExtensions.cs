using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BoomaEcommerce.Core
{
    public static class LinqExtensions
    {
        public static (IEnumerable<T> satisfiedPred, IEnumerable<T> notSatisfiedPred) Split<T>(
            this IEnumerable<T> enumerable,
            Func<T, bool> pred)
        {
            var split = enumerable.ToLookup(pred);
            return (split[true], split[false]);
        }
    }
}
