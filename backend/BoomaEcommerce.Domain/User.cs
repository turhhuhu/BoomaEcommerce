using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BoomaEcommerce.Core;
using Microsoft.AspNetCore.Identity;

namespace BoomaEcommerce.Domain
{
    public class User : IdentityUser<Guid>, IBaseEntity
    {
       
        public Guid Guid
        {
            get => Id;
            set => Id = value;
        }
        public string Name { get; set; }
        public string LastName { get; set; }
        public DateTime DateOfBirth { get; set; }
        public ICollection<Notification> Notifications { get; set; }

        public User()
        {
            Notifications = new List<Notification>();
            Guid = Guid.NewGuid();
        }
        public void AddNotification(Notification notification)
        {
            Notifications.Add(notification);
        }
    }
}
