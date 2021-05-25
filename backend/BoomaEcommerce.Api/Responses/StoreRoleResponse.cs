using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BoomaEcommerce.Services.DTO;

namespace BoomaEcommerce.Api.Responses
{
    public abstract class StoreRoleResponse
    {
        public Guid Guid { get; set; }
        public abstract RoleType Type { get;}
        public StoreMetaData StoreMetaData { get; set; }
        public UserMetaData UserMetaData { get; set; }
    }

    public class OwnerShipRoleResponse : StoreRoleResponse
    {
        public const RoleType OwnershipType = RoleType.Ownership;
        public override RoleType Type { get; } = OwnershipType;
    }
    public class ManagementRoleResponse : StoreRoleResponse
    {
        public const RoleType ManagementType = RoleType.Management;
        public override RoleType Type { get; } = ManagementType;
        public StoreManagementPermissionsDto Permissions { get; set; }
    }

    public enum RoleType
    {
        Ownership,
        Management
    }
}
