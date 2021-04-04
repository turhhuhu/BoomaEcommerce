using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BoomaEcommerce.Domain
{
    public class StoreOwnership : StoreSeller
    {
        public Store Store { get; set; }
        public List<StoreManagementPermission> Permissions { get; set; }
    }
}
