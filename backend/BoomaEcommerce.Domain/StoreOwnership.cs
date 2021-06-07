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

        private ConcurrentDictionary<Guid, StoreOwnership> _storeOwnerships;
        private ConcurrentDictionary<Guid, StoreManagement> _storeManagements;

        public List<StoreOwnership> StoreOwnerships
        {
            get => _storeOwnerships.Values.ToList();
            set => _storeOwnerships = new ConcurrentDictionary<Guid, StoreOwnership>(value.ToDictionary(o => o.Guid));
        }
        public List<StoreManagement> StoreManagements
        {
            get => _storeManagements.Values.ToList();
            set => _storeManagements = new ConcurrentDictionary<Guid, StoreManagement>(value.ToDictionary(o => o.Guid));
        }

        /*
        public ICollection<StoreOwnership> StoreOwnerships
        {
            get => _storeOwnerships.Values;
            set => _storeOwnerships = new ConcurrentDictionary<Guid, StoreOwnership>(value.ToDictionary(o => o.Guid));
        }
        public ICollection<StoreManagement> StoreManagements
        {
            get => _storeManagements.Values;
            set => _storeManagements = new ConcurrentDictionary<Guid, StoreManagement>(value.ToDictionary(o => o.Guid));
        }
        */
        public StoreOwnership()
        {
            _storeManagements = new ConcurrentDictionary<Guid, StoreManagement>();
            _storeOwnerships = new ConcurrentDictionary<Guid, StoreOwnership>();
        }

        public void AddOwner(StoreOwnership owner)
        {
            _storeOwnerships.TryAdd(owner.Guid, owner);
        }
        public void AddManager(StoreManagement manager)
        {
            _storeManagements.TryAdd(manager.Guid, manager);
        }


        public bool RemoveManager(Guid managerToRemove)
        {
            return _storeManagements.TryRemove(managerToRemove, out _);
        }


        public (List<StoreOwnership>, List<StoreManagement>) GetSubordinates(int? level = null)
        {
            if (level > 0 || !level.HasValue)
            {
                var sellers = _storeOwnerships.Values.Select(owner => owner.GetSubordinates(level - 1)).ToList();
                var owners = sellers.SelectMany(pair => pair.Item1).Concat(_storeOwnerships.Values).ToList();
                var managers = sellers.SelectMany(pair => pair.Item2).Concat(_storeManagements.Values).ToList();
                return (owners, managers);
            }

            return (_storeOwnerships.Values.ToList(), _storeManagements.Values.ToList());
        }

        public void RemoveOwner(Guid ownershipGuid)
        {
            var owner = GetOwner(ownershipGuid);
            if (owner == null)
            {
                return;
            }
            owner.RemoveSubordinatesRecursively();
            _storeOwnerships.TryRemove(ownershipGuid, out _);
        }

        public void RemoveSubordinatesRecursively()
        {
            this._storeManagements.Clear(); // Remove all managers 
            foreach (var (guid, ownership) in this._storeOwnerships) // Call Recursively 
            { 
                ownership.RemoveSubordinatesRecursively();
            }
            this._storeOwnerships.Clear(); // Remove all owners 
        }

        public StoreOwnership GetOwner(Guid ownerGuid)
        {
            return _storeOwnerships.TryGetValue(ownerGuid, out var ownership) 
                ? ownership 
                : null;
        }

        public bool ContainsManagement(Guid managementGuid)
        {
            return _storeManagements.ContainsKey(managementGuid);
        }
        public bool ContainsOwnership(Guid ownershipGuid)
        {
            return _storeOwnerships.ContainsKey(ownershipGuid);
        }

    }
    
    
    
    
}
