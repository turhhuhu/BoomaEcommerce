#nullable enable
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using BoomaEcommerce.Services.DTO;

namespace BoomaEcommerce.Api.Requests
{
    public class CreateRoleRequest
    {
        protected CreateRoleRequest() { }
        public Guid? NominatedUserGuid { get; set; }
        public string? NominatedUserName { get; set; }
        public Guid NominatingOwnershipGuid { get; set; }
    }

    public class CreateOwnershipRequest : CreateRoleRequest
    {

    }
    public class CreateManagementRequest : CreateRoleRequest
    {
        protected CreateManagementRequest(StoreManagementPermissionsDto permissions)
        {
            Permissions = permissions;
        }

        public StoreManagementPermissionsDto Permissions { get; set; }

    }
}
