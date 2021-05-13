using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BoomaEcommerce.Api.Responses
{
    public class StoreSellersResponse
    {
        public List<ManagementRoleResponse> StoreManagers { get; set; }
        public List<OwnerShipRoleResponse> StoreOwners { get; set; }
    }
}
