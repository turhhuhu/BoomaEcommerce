﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BoomaEcommerce.Services.DTO
{
    public class ProductDto : BaseEntityDto
    {
        public string Name { get; set; }
        public StoreDto Store { get; init; }
        public string Category { get; set; }
        public double? Price { get; set; }
        public int? Amount { get; set; }
    }
}
