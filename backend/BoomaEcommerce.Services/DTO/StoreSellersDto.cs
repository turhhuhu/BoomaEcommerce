using System.Collections.Generic;

namespace BoomaEcommerce.Services.DTO
{
    public class StoreSellersDto
    {
        public List<StoreManagementDto> StoreManagers { get; set; }
        public List<StoreOwnershipDto> StoreOwners { get; set; }
        public StoreSellersDto(List<StoreOwnershipDto> owners, List<StoreManagementDto> managers)
        {
            StoreManagers = managers;
            StoreOwners = owners;
        }

        public StoreSellersDto()
        {
            StoreManagers = new List<StoreManagementDto>();
            StoreOwners = new List<StoreOwnershipDto>();
        }

    }
}