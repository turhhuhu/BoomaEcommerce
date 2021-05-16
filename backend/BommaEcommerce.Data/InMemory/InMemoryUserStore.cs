using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using BoomaEcommerce.Domain;
using Microsoft.AspNetCore.Identity;

namespace BoomaEcommerce.Data.InMemory
{
    public class InMemoryUserStore : IUserStore<User>, IUserPasswordStore<User>, IUserRoleStore<User>
    {
        public static Dictionary<string, User> Users = new ();

        public static Dictionary<Guid, string> PasswordHash = new();

        public static Dictionary<Guid, ConcurrentDictionary<string, byte>> Roles { get; set; } = new();

        public void Dispose()
        {
            
        }

        public Task<string> GetUserIdAsync(User user, CancellationToken cancellationToken)
        {
            return Task.FromResult(Users[user.Guid.ToString()].Guid.ToString());
        }

        public Task<string> GetUserNameAsync(User user, CancellationToken cancellationToken)
        {
            if (Users.TryGetValue(user.Guid.ToString(), out var foundUser))
            {
                return Task.FromResult(foundUser.UserName);
            }

            return Task.FromResult<string>(null);
        }

        public Task SetUserNameAsync(User user, string userName, CancellationToken cancellationToken)
        {
            Users[user.Guid.ToString()].UserName = userName;
            return Task.CompletedTask;
        }

        public Task<string> GetNormalizedUserNameAsync(User user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.UserName);
        }

        public Task SetNormalizedUserNameAsync(User user, string normalizedName, CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        public Task<IdentityResult> CreateAsync(User user, CancellationToken cancellationToken)
        {
            return Task.FromResult(AddUserByIdentifiers(user)
                ? IdentityResult.Success 
                : IdentityResult.Failed(new IdentityError {Description = "User already exists."}));
        }

        private bool AddUserByIdentifiers(User user)
        {
            return Users.TryAdd(user.Guid.ToString(), user) 
                   && Users.TryAdd(user.UserName.ToUpper(), user)
                   && Roles.TryAdd(user.Guid, new ConcurrentDictionary<string, byte>());
        }

        public Task<IdentityResult> UpdateAsync(User user, CancellationToken cancellationToken)
        {
            if (!Users.ContainsKey(user.Guid.ToString())) return Task.FromResult(IdentityResult.Failed());

            Users[user.Guid.ToString()] = user;
            return Task.FromResult(IdentityResult.Success);
        }

        public Task<IdentityResult> DeleteAsync(User user, CancellationToken cancellationToken)
        {
            return Task.FromResult(Users.Remove(user.Guid.ToString(), out _) 
                ? IdentityResult.Success 
                : IdentityResult.Failed());
        }

        public Task<User> FindByIdAsync(string userId, CancellationToken cancellationToken)
        {
            return Users.TryGetValue(userId, out var user) 
                ? Task.FromResult(user) 
                : Task.FromResult<User>(null);
        }

        public Task<User> FindByNameAsync(string normalizedUserName, CancellationToken cancellationToken)
        {
            return Users.TryGetValue(normalizedUserName, out var user)
                ? Task.FromResult(user)
                : Task.FromResult<User>(null);
        }

        public Task SetPasswordHashAsync(User user, string passwordHash, CancellationToken cancellationToken)
        {
            PasswordHash[user.Guid] = passwordHash;
            return Task.CompletedTask;
        }

        public Task<string> GetPasswordHashAsync(User user, CancellationToken cancellationToken)
        {
            return Task.FromResult(PasswordHash[user.Guid]);
        }

        public Task<bool> HasPasswordAsync(User user, CancellationToken cancellationToken)
        {
            return Task.FromResult(PasswordHash.ContainsKey(user.Guid));
        }

        public Task AddToRoleAsync(User user, string roleName, CancellationToken cancellationToken)
        {
            if (Roles.TryGetValue(user.Guid, out var roles))
            {
                roles.TryAdd(roleName, byte.MinValue);
            }

            return Task.CompletedTask;
        }

        public Task RemoveFromRoleAsync(User user, string roleName, CancellationToken cancellationToken)
        {
            if (Roles.TryGetValue(user.Guid, out var roles))
            {
                roles.TryRemove(roleName, out _);
            }

            return Task.CompletedTask;
        }

        public Task<IList<string>> GetRolesAsync(User user, CancellationToken cancellationToken)
        {
            return Task.FromResult<IList<string>>(Roles.TryGetValue(user.Guid, out var roles) 
                ? roles.Keys.ToList() 
                : new List<string>());
        }

        public Task<bool> IsInRoleAsync(User user, string roleName, CancellationToken cancellationToken)
        {
            return Task.FromResult(Roles.TryGetValue(user.Guid, out var roles) && roles.ContainsKey(roleName));
        }

        public Task<IList<User>> GetUsersInRoleAsync(string roleName, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
