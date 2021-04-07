using System.Collections.Generic;
using BoomaEcommerce.Domain;

namespace BoomaEcommerce.Services.DTO
{
    public class StoreSellersResponse
    {
        public List<StoreManagementDto> StoreManagers { get; set; }
        public List<StoreOwnershipDto> StoreOwners { get; set; }
    }
}