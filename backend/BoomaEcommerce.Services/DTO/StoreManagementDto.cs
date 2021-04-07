using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BoomaEcommerce.Services.DTO
{
    public class StoreManagementDto : BaseEntityDto
    {
        public StoreDto Store { get; set; }
        public UserDto User { get; set; }
    }
}
