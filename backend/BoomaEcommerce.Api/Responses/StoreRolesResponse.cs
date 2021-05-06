using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BoomaEcommerce.Services.DTO;

namespace BoomaEcommerce.Api.Responses
{
    public class StoreRolesResponse
    {
        public IEnumerable<StoreOwnershipDto> OwnerFounderRoles { get; set; }
        public IEnumerable<StoreOwnershipDto> OwnerNotFounderRoles { get; set; }
        public IReadOnlyCollection<StoreManagementDto> ManagerRoles { get; set; }
    }
}
