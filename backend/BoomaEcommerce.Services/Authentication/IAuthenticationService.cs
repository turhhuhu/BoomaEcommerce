using BoomaEcommerce.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BoomaEcommerce.Services
{
    public interface IAuthenticationService
    {
        Task RegisterAsync(string username, string password);
        Task<AuthenticationToken> LoginAsync(string username, string password);
    }
}
