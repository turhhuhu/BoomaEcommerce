using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using BoomaEcommerce.Services.SwaggerFilters;

namespace BoomaEcommerce.Services.DTO
{
    public class ProductDto : BaseEntityDto
    {
        public string Name { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public Guid StoreGuid { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        [SwaggerExclude]
        public StoreMetaData StoreMetaData { get; set; }
        public string Category { get; set; }
        public decimal? Price { get; set; }
        public int? Amount { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public decimal? Rating { get; set; }
    }
}
