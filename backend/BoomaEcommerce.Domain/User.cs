using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BoomaEcommerce.Core;

namespace BoomaEcommerce.Domain
{
    public class User : BaseEntity
    {
        public string UserName { get; set; }
        public User(string userName, Guid guid)
        {
            this.UserName = userName;
            this.Guid = guid;
        }

        public string Name { get; set; }
        public string LastName { get; set; }

    }
}
