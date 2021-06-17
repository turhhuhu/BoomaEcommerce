using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BoomaEcommerce.Core
{
    public static class EqualityComparers
    {
        public class SameGuid<T> : EqualityComparer<T>
            where T : BaseEntity
        {
            public override bool Equals(T x, T y)
            {
                return x?.Guid == y?.Guid;
            }

            public override int GetHashCode(T obj)
            {
                return obj.Guid.GetHashCode();
            }
        }
    }
}
