using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BoomaEcommerce.Core;

namespace BoomaEcommerce.Domain
{
    public class StoreManagementPermission : BaseEntity
    {
        public StoreManagement StoreManagement { get; set; }

        // Example flag of permission.
        public bool CanDoSomething { get; set; }
    }
}
