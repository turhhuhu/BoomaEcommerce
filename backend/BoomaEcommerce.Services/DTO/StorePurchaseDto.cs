using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BoomaEcommerce.Services.DTO
{
    public class StorePurchaseDto : BaseEntityDto
    {
        public List<PurchaseProductDto> ProductsPurchases { get; set; }
        public UserDto Buyer { get; set; }
        public StoreDto Store { get; set; }
        public double TotalPrice { get; set; }
    }
}
