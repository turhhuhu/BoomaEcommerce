using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BoomaEcommerce.Services.DTO.Discounts;

namespace BoomaEcommerce.Services.DTO.Policies
{
    public class CreateDiscountDto
    {
        public DiscountDto DiscountToCreate { get; set; }
    }
}