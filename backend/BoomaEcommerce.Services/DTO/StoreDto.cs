using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BoomaEcommerce.Services.DTO
{
    public class StoreDto : BaseEntityDto
    {
        public string StoreName { get; set; }
        public string Description { get; set; }
        public UserDto StoreFounder { get; set; }
        public float? Rating { get; set; }
    }
}
