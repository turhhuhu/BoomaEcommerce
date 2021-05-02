using BoomaEcommerce.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BoomaEcommerce.Services.DTO;

namespace BoomaEcommerce.Services.Authentication
{
    public interface IAuthenticationService
    {
        Task<AuthenticationResult> RegisterAsync(UserDto userDto, string password);
        Task<AuthenticationResult> LoginAsync(string username, string password);
        Task<AuthenticationResult> RefreshJwtToken(string token, string refreshToken);

        Task<AuthenticationResult> RegisterAdminAsync(AdminUserDto userDto, string password);
    }
}
