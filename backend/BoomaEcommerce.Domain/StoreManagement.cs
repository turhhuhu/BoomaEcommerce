using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BoomaEcommerce.Core;

namespace BoomaEcommerce.Domain
{
    public class StoreManagement : BaseEntity
    {
        public StoreManagement()
        {
            Permissions = new StoreManagementPermission { Guid = this.Guid };
        }
        public User User { get; set; }
        public Store Store { get; set; }
        public StoreManagementPermission Permissions { get; set; }
    }

}
