using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BoomaEcommerce.Services.DTO
{
    public class PurchaseProductDto : BaseEntityDto
    {
        public ProductDto ProductDto { get; set; }
        public int Amount { get; set; }
    }
}
