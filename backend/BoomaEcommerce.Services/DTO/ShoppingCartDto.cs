using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BoomaEcommerce.Services.DTO
{
    public class ShoppingCartDto : BaseEntityDto
    {
        public IReadOnlyCollection<ShoppingBasketDto> Baskets { get; set; }

    }
}
