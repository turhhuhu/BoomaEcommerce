using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BoomaEcommerce.Domain;

namespace BoomaEcommerce.Services.Authentication
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly ILogger<AuthenticationService> _logger;
        private readonly UserManager<User> _userManager;
        public AuthenticationService(ILogger<AuthenticationService> logger, UserManager<User> userManager)
        {
            _logger = logger;
            _userManager = userManager;
        }
        public Task<AuthenticationToken> LoginAsync(string username, string password)
        {
            throw new NotImplementedException();
        }

        public Task RegisterAsync(string username, string password)
        {
            throw new NotImplementedException();
        }
    }
}
