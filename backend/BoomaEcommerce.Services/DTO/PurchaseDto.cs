using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BoomaEcommerce.Services.DTO
{
    public class PurchaseDto : BaseEntityDto
    {
        public List<PurchaseProductDto> ProductsPurchases { get; set; }
        public UserDto Buyer { get; set; }
        public double TotalPrice { get; set; }
    }
}
