using System.Collections.Generic;

namespace BoomaEcommerce.Services.DTO
{
    public class StoreSellersResponse
    {
        public StoreSellersResponse(List<StoreOwnershipDto> owners, List<StoreManagementDto> managers)
        {
        }

        public List<StoreManagementDto> StoreManagers { get; set; }
        public List<StoreOwnershipDto> StoreOwners { get; set; }
    }
}