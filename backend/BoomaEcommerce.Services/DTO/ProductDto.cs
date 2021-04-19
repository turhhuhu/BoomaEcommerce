using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BoomaEcommerce.Services.DTO
{
    public class ProductDto : BaseEntityDto
    {
        public string Name { get; set; }
        public StoreDto Store { get; set; }
        public string Category { get; set; }
        public decimal? Price { get; set; }
        public int? Amount { get; set; }
        public float? Rating { get; set; }
    }
}
