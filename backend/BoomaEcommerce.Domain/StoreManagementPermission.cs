using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BoomaEcommerce.Core;

namespace BoomaEcommerce.Domain
{
    public class StoreManagementPermission : BaseEntity
    {
        public bool CanAddProduct { get; set; } = true;
        public bool CanDeleteProduct { get; set; } = true;
        public bool CanUpdateProduct { get; set; } = true;
        public bool CanGetSellersInfo { get; set; } = true;
    }
}
