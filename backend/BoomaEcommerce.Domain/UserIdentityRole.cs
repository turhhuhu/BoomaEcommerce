using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace BoomaEcommerce.Domain
{
    public class UserIdentityRole : IdentityRole<Guid>
    {
        public UserIdentityRole(string roleName) : base(roleName)
        {
            
        }

        public UserIdentityRole()
        {
            
        }
    }
}
