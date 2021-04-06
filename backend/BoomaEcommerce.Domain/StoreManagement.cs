using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BoomaEcommerce.Core;

namespace BoomaEcommerce.Domain
{
    public class StoreManagement : BaseEntity
    {
        public User User { get; set; }
        public Store Store { get; set; }
    }
}
