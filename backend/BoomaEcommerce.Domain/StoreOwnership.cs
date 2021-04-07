using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.InteropServices;
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

        public (List<StoreOwnership>, List<StoreManagement>) GetSubordinates()
        {
            var sellers = StoreOwnerships.Values.Select(owner => owner.GetSubordinates()).ToList();
            var owners = sellers.SelectMany(pair => pair.Item1).ToList();
            var managers = sellers.SelectMany(pair => pair.Item2).ToList();
            return (owners, managers);
        }
    }
    
    
    
    
}
