using System.Collections.Generic;

namespace BoomaEcommerce.Services.DTO
{
    public class StorePurchaseDto : BaseEntityDto
    {
        public List<PurchaseProductDto> PurchaseProducts { get; set; }
        public UserDto Buyer { get; set; }
        public StoreDto Store { get; set; }
        public decimal TotalPrice { get; set; }
    }
}
