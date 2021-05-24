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

        public bool RemoveManager(Guid managerToRemove)
        {
            return StoreManagements.TryRemove(managerToRemove, out _);
        }


        public (List<StoreOwnership>, List<StoreManagement>) GetSubordinates(int? level = null)
        {
            if (level > 0)
            {
                var sellers = StoreOwnerships.Values.Select(owner => owner.GetSubordinates()).ToList();
                var owners = sellers.SelectMany(pair => pair.Item1).Concat(StoreOwnerships.Values).ToList();
                var managers = sellers.SelectMany(pair => pair.Item2).Concat(StoreManagements.Values).ToList();
                return (owners, managers);
            }

            return (StoreOwnerships.Values.ToList(), StoreManagements.Values.ToList());
        }

        public void RemoveOwner()
        {
            this.StoreManagements.Clear(); // Remove all managers 
            foreach (var (guid, ownership) in this.StoreOwnerships) // Call Recursively 
            { 
                ownership.RemoveOwner();
            }
            this.StoreOwnerships.Clear(); // Remove all owners 
        }
    }
    
    
    
    
}
