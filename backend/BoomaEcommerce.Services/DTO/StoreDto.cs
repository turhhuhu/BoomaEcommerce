using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace BoomaEcommerce.Services.DTO
{
    public class StoreDto : BaseEntityDto
    {
        public string StoreName { get; set; }
        public string Description { get; set; }
        public Guid FounderUserGuid { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public float? Rating { get; set; }
    }
}
