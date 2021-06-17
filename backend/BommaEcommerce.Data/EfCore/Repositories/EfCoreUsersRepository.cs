using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BoomaEcommerce.Domain;
using Microsoft.EntityFrameworkCore;

namespace BoomaEcommerce.Data.EfCore.Repositories
{
    public class EfCoreUsersRepository : EfCoreRepository<User>
    {
        public EfCoreUsersRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }

        public override Task<User> FindByIdAsync(Guid guid)
        {
            return DbContext.Set<User>()
                .Include(u => u.Notifications)
                    .ThenInclude((Notification n) => (n as StorePurchaseNotification).Store)
                .Include(u => u.Notifications)
                    .ThenInclude((Notification n) => (n as StorePurchaseNotification).Buyer)
                .Include(u => u.Notifications)
                    .ThenInclude((Notification n) => (n as RoleDismissalNotification).Store)
                .Include(u => u.Notifications)
                    .ThenInclude((Notification n) => (n as RoleDismissalNotification).DismissingUser)
                .FirstOrDefaultAsync(o => o.Guid == guid);
        }
    }
}
