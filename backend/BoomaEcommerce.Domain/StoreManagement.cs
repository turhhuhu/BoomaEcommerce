using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BoomaEcommerce.Domain
{
    public class StoreManagement : BaseEntity
    {
        public User User { get; set; }
        public Store Store { get; set; }
        public List<StoreManagementPermission> Permissions { get; set; }


        public async Task<bool> AddPermissions(List<StoreManagementPermission> permissions)
        {
            Permissions.AddRange(permissions);
            Permissions.Distinct();
            return true;
        }

        public async Task<bool> RemovePermissions(List<StoreManagementPermission> permissions)
        {
            foreach (var per in permissions)
            {
                var succRemove = Permissions.Remove(per);
                if (!succRemove) return false;
            }
            Permissions.Distinct();
            return true;
        }
    }

}
