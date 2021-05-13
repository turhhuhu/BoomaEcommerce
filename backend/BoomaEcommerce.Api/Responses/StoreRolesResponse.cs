using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BoomaEcommerce.Services.DTO;

namespace BoomaEcommerce.Api.Responses
{
    public class StoreRolesResponse
    {
        public IEnumerable<OwnerShipRoleResponse> OwnerFounderRoles { get; set; }
        public IEnumerable<OwnerShipRoleResponse> OwnerNotFounderRoles { get; set; }
        public IEnumerable<ManagementRoleResponse> ManagerRoles { get; set; }
    }
}
