using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BoomaEcommerce.Domain
{
    public class User : BaseEntity
    {
        public string UserName { get; set; }
        public Guid Id { get; set; }
        public User(string userName, Guid guid)
        {
            this.UserName = userName;
            this.Id = guid;
        }

    }
}
