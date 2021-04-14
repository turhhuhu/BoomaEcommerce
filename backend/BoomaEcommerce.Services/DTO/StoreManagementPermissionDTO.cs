using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BoomaEcommerce.Domain;

namespace BoomaEcommerce.Services.DTO
{
    public class StoreManagementPermissionDto : BaseEntityDto
    {
        public bool CanDoSomething { get; set; }
        public bool ExampleFlag1 { get; set; }
        public bool ExampleFlag2 { get; set; }

        public bool CanAddProduct { get; set; }
        public bool CanDeleteProduct { get; set; }
        public bool CanUpdateProduct { get; set; }

        //public StoreManagementDto StoreManagement { get; set; }
    }
}
