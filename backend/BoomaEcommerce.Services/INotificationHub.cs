using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BoomaEcommerce.Domain;
using BoomaEcommerce.Services.DTO;

namespace BoomaEcommerce.Services
{
    public interface INotificationHub
    {
        Task NotifyAsync(NotificationDto notification, Guid userToNotify);
        Task NotifyManyAsync(NotificationDto notification, IEnumerable<Guid> usersToNotify);
    }
}
