using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BoomaEcommerce.Services.DTO
{
    public class ShoppingCartDto : BaseEntityDto, IUserRelatedResource
    {
        public UserDto User { get; set; }
        public IDictionary<Guid, ShoppingBasketDto> Baskets { get; set; }
    }
}
