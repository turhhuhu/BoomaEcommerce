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


        public ISet<StoreOwnership> StoreOwnerships { get; set; }
        public ISet<StoreManagement> StoreManagements { get; set; }

        public StoreOwnership()
        {
            StoreOwnerships = new HashSet<StoreOwnership>(new EqualityComparers.SameGuid<StoreOwnership>());
            StoreManagements = new HashSet<StoreManagement>(new EqualityComparers.SameGuid<StoreManagement>());
        }

        public void AddOwner(StoreOwnership owner)
        {
            StoreOwnerships.Add(owner);
        }
        public void AddManager(StoreManagement manager)
        {
            StoreManagements.Add(manager);
        }


        public bool RemoveManager(Guid managerToRemove)
        {
            return StoreManagements.Remove(new StoreManagement {Guid = managerToRemove});
        }


        public (List<StoreOwnership>, List<StoreManagement>) GetSubordinates(int? level = null)
        {
            if (level > 0 || !level.HasValue)
            {
                var sellers = StoreOwnerships.Select(owner => owner.GetSubordinates(level - 1)).ToList();
                var owners = sellers.SelectMany(pair => pair.Item1).Concat(StoreOwnerships).ToList();
                var managers = sellers.SelectMany(pair => pair.Item2).Concat(StoreManagements).ToList();
                return (owners, managers);
            }

            return (StoreOwnerships.ToList(), StoreManagements.ToList());
        }

        public void RemoveOwner(Guid ownershipGuid)
        {
            var owner = GetOwner(ownershipGuid);
            if (owner == null)
            {
                return;
            }
            owner.RemoveSubordinatesRecursively();
            StoreOwnerships.Remove(new StoreOwnership {Guid = ownershipGuid});
        }

        public void RemoveSubordinatesRecursively()
        {
            this.StoreManagements.Clear(); // Remove all managers 
            foreach (var ownership in StoreOwnerships) // Call Recursively 
            { 
                ownership.RemoveSubordinatesRecursively();
            }
            this.StoreOwnerships.Clear(); // Remove all owners 
        }

        public StoreOwnership GetOwner(Guid ownerGuid)
        {
            return StoreOwnerships.FirstOrDefault(o => o.Guid == ownerGuid);
        }

        public bool ContainsManagement(Guid managementGuid)
        {
            return StoreManagements.Contains(new StoreManagement {Guid = managementGuid});
        }
        public bool ContainsOwnership(Guid ownershipGuid)
        {
            return StoreOwnerships.Contains(new StoreOwnership { Guid = ownershipGuid });
        }

    }

}
