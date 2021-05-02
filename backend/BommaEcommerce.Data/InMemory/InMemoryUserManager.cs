using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using BoomaEcommerce.Domain;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace BoomaEcommerce.Data.InMemory
{
    public class InMemoryUserManager : UserManager<User>
    {
        public InMemoryUserStore UserStore { get; set; }
        public CancellationToken CancelToken = CancellationToken.None;
        public InMemoryUserManager(InMemoryUserStore store) : base(store, null , null, null, null, null, null,null, null)
        {
            UserStore = store;
        }

        public override Task<User> FindByNameAsync(string userName)
        {
            return UserStore.FindByNameAsync(userName, CancelToken);
        }

        public override async Task<IdentityResult> CreateAsync(User user, string password)
        {
            await UserStore.SetPasswordHashAsync(user, password, CancelToken);
            return await UserStore.CreateAsync(user, CancelToken);
        }
    }
}
