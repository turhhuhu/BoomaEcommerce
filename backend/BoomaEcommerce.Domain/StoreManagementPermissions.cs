using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BoomaEcommerce.Core;

namespace BoomaEcommerce.Domain
{
    public class StoreManagementPermissions : BaseEntity
    {
        public bool CanAddProduct { get; set; } = true;
        public bool CanDeleteProduct { get; set; } = true;
        public bool CanUpdateProduct { get; set; } = true;
        public bool CanGetSellersInfo { get; set; } = true;
        public bool CanCreatePolicy { get; set; } = true;
        public bool CanDeletePolicy { get; set; } = true;
        public bool CanGetPolicyInfo { get; set; } = true;
        public bool CanUpdatePolicyInfo { get; set; } = true;
        public bool CanDeleteDiscount { get; set; } = true;
        public bool CanGetDiscountInfo { get; set; } = true;
        public bool CanCreateDiscounts { get; set; } = true;
    }


}
