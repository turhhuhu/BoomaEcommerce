using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BoomaEcommerce.Core;

namespace BoomaEcommerce.Domain
{
    public class StoreManagementPermission : BaseEntity
    {
        public bool CanAddProduct { get; set; }
        public bool CanDeleteProduct { get; set; }
        public bool CanUpdateProduct { get; set; }
        public bool CanGetSellersInfo { get; set; }
    }
}
