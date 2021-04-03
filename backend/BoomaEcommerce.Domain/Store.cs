using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BoomaEcommerce.Domain
{
    public class Store : BaseEntity
    {
        
        public Store(Guid guid)
        {
            this.Guid = guid;
        }
    }
}
