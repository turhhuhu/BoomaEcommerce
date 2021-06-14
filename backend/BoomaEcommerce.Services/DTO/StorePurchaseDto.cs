using System;
using System.Collections.Generic;
using BoomaEcommerce.Services.SwaggerFilters;

namespace BoomaEcommerce.Services.DTO
{
    public class StorePurchaseDto : BaseEntityDto
    {
        public List<PurchaseProductDto> PurchaseProducts { get; set; }
        public Guid BuyerGuid { get; set; }

        [SwaggerExclude]
        public UserMetaData UserMetaData { get; set; }
        public Guid StoreGuid { get; set; }

        [SwaggerExclude]
        public StoreMetaData StoreMetaData { get; set; }
        public decimal TotalPrice { get; set; }
    }
}
