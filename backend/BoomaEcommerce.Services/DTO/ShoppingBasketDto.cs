using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BoomaEcommerce.Services.DTO
{
    public class ShoppingBasketDto : BaseEntityDto
    {
        public StoreDto Store { get; set; }
        public List<PurchaseProductDto> PurchaseProductDtos { get; set; }
    }
}
