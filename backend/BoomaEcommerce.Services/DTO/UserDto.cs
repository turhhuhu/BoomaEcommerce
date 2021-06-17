using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BoomaEcommerce.Services.DTO
{
    public class UserDto
    {
        public Guid Guid { get; set; }
        public string UserName { get; set; }
        public string Name { get; set; }
        public string LastName { get; set; }

        public DateTime DateOfBirth { get; set; }
        public ICollection<NotificationDto> Notifications { get; set; }
    }
}
