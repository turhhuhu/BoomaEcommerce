using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BoomaEcommerce.Domain
{
    public class Store
    {
        public Guid Id { get; set; }

        public Store(Guid storeGuid)
        {
            Id = storeGuid;
        }
    }
}
