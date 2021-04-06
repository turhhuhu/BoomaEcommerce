using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BoomaEcommerce.Core;

namespace BoomaEcommerce.Domain
{
    public class StoreOwnership : BaseEntity
    {
        public Store Store { get; set; }
        public User User { get; set; }

        public ConcurrentDictionary<Guid, StoreOwnership> StoreOwnerships { get; set; } = new();
        public ConcurrentDictionary<Guid, StoreManagement> StoreManagements { get; set; } = new();
    }
}
