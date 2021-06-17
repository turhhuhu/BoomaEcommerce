using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BoomaEcommerce.Services.SwaggerFilters;

namespace BoomaEcommerce.Services.DTO
{
    public class PurchaseProductDto : BaseEntityDto
    {
        public Guid ProductGuid { get; set; }
        [SwaggerExclude]
        public ProductMetaData ProductMetaData { get; set; }
        public int Amount { get; set; }
        
        public decimal Price { get; set; }
    }
}
