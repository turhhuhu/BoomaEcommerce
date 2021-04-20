using System.Collections.Generic;

namespace BoomaEcommerce.Services.DTO
{
    public class StoreSellersResponse
    {
        public List<StoreManagementDto> StoreManagers { get; set; }
        public List<StoreOwnershipDto> StoreOwners { get; set; }
        public StoreSellersResponse(List<StoreOwnershipDto> owners, List<StoreManagementDto> managers)
        {
            StoreManagers = managers;
            StoreOwners = owners;
        }

        public StoreSellersResponse()
        {
            StoreManagers = new List<StoreManagementDto>();
            StoreOwners = new List<StoreOwnershipDto>();
        }

    }
}