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
            if (level > 0 || !level.HasValue)
            {
                var sellers = StoreOwnerships.Values.Select(owner => owner.GetSubordinates(level - 1)).ToList();
                var owners = sellers.SelectMany(pair => pair.Item1).Concat(StoreOwnerships.Values).ToList();
                var managers = sellers.SelectMany(pair => pair.Item2).Concat(StoreManagements.Values).ToList();
                return (owners, managers);
            }

            return (StoreOwnerships.Values.ToList(), StoreManagements.Values.ToList());
        }

        public void RemoveOwner(Guid ownershipGuid)
        {
            var owner = GetOwner(ownershipGuid);
            if (owner == null)
            {
                return;
            }
            owner.RemoveSubordinatesRecursively();
            StoreOwnerships.TryRemove(ownershipGuid, out _);
        }

        public void RemoveSubordinatesRecursively()
        {
            this.StoreManagements.Clear(); // Remove all managers 
            foreach (var (guid, ownership) in this.StoreOwnerships) // Call Recursively 
            { 
                ownership.RemoveSubordinatesRecursively();
            }
            this.StoreOwnerships.Clear(); // Remove all owners 
        }

        public StoreOwnership GetOwner(Guid ownerGuid)
        {
            return StoreOwnerships.TryGetValue(ownerGuid, out var ownership) 
                ? ownership 
                : null;
        }
    }
    
    
    
    
}
