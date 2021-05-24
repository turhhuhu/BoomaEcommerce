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
        public abstract string Type { get;}
        public StoreMetaData StoreMetaData { get; set; }
        public UserMetaData UserMetaData { get; set; }
    }

    public class OwnerShipRoleResponse : StoreRoleResponse
    {
        public const string OwnershipType = "ownership";
        public override string Type { get; } = OwnershipType;
    }
    public class ManagementRoleResponse : StoreRoleResponse
    {
        public const string ManagementType = "management";
        public override string Type { get; } = ManagementType;
        public StoreManagementPermissionsDto Permissions { get; set; }
    }
}
