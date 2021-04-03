using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BoomaEcommerce.Domain
{
    public class StoreOwnership
    {
        public User User { get; set; }
        public Store Store { get; set; }
        public List<StoreManagementPermission> Permissions { get; set; }
    }
}
