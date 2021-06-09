using System;
using System.Collections.Generic;

namespace BoomaEcommerce.Services.DTO
{
    public class StorePurchaseDto : BaseEntityDto
    {
        public List<PurchaseProductDto> PurchaseProducts { get; set; }
        public Guid BuyerGuid { get; set; }
        public Guid StoreGuid { get; set; }
        public decimal TotalPrice { get; set; }
    }
}
