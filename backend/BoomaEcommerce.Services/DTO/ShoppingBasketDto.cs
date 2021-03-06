using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BoomaEcommerce.Services.DTO
{
    public class ShoppingBasketDto : BaseEntityDto
    {
        public Guid StoreGuid { get; set; }
        public List<PurchaseProductDto> PurchaseProducts { get; set; }
    }
}
