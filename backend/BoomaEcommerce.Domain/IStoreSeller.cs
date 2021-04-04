using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BoomaEcommerce.Domain
{
    public class StoreSeller : BaseEntity
    {
        public User User { get; set; }
    }
}
