using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BoomaEcommerce.Services.DTO
{
    public class StoreMetaData
    {
        public Guid StoreGuid { get; set; }
        public string StoreName { get; set; }
        public string Description { get; set; }
    }

    public class UserMetaData
    {
        public Guid UserGuid { get; set; }
        public string UserName { get; set; }
    }

    public class ProductMetaData
    {
        public Guid ProductGuid { get; set; }
        public string ProductName { get; set; }
    }
}
