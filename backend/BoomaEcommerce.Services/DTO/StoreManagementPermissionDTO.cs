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
        public StoreManagementDto SmDto { get; set; }
    }
}
