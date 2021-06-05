using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BoomaEcommerce.Domain;

namespace BoomaEcommerce.Services.DTO
{
    public class StoreManagementPermissionsDto : BaseEntityDto
    {
        public bool CanAddProduct { get; set; }
        public bool CanDeleteProduct { get; set; }
        public bool CanUpdateProduct { get; set; }
        public bool CanGetSellersInfo { get; set; }
        public bool CanCreatePolicy { get; set; }
        public bool CanDeletePolicy { get; set; }
        public bool CanDeleteDiscount { get; set; }
        public bool CanGetPolicyInfo { get; set; }
        public bool CanGetDiscountInfo { get; set; }
        public bool CanUpdatePolicyInfo { get; set; }
        public bool CanCreateDiscounts { get; set; }
    }
}
