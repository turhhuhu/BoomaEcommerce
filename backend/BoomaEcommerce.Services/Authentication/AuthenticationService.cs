using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using BoomaEcommerce.Domain;
using BoomaEcommerce.Services.DTO;
using BoomaEcommerce.Services.Settings;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;


namespace BoomaEcommerce.Services.Authentication
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly ILogger<AuthenticationService> _logger;
        private readonly UserManager<User> _userManager;
        private readonly JwtSettings _jwtSettings;

        public AuthenticationService(ILogger<AuthenticationService> logger,
            UserManager<User> userManager,
            JwtSettings jwtSettings)
        {
            _logger = logger;
            _userManager = userManager;
            _jwtSettings = jwtSettings;
        }
        public AuthenticationService(ILogger<AuthenticationService> logger,
            UserManager<User> userManager,
            IOptions<JwtSettings> jwtOptions) : this(logger, userManager, jwtOptions.Value) { }

        public async Task<AuthenticationResponse> LoginAsync(string username, string password)
        {
            var existingUser = await _userManager.FindByNameAsync(username);

            if (existingUser == null)
            {
                return new AuthenticationResponse
                {
                    Errors = new[] { "Username doesn't exist." }
                };
            }

            var userHasValidPassword = await _userManager.CheckPasswordAsync(existingUser, password);

            if (!userHasValidPassword)
            {
                return new AuthenticationResponse
                {
                    Errors = new[] { "Bad username or password." }
                };
            }

            return new AuthenticationResponse
            {
                Success = true,
                Token = GenerateToken(existingUser)
            };
        }

        public async Task<AuthenticationResponse> RegisterAsync(string username, string password)
        {

            var existingUser = await _userManager.FindByNameAsync(username);

            if (existingUser != null)
            {
                return new AuthenticationResponse
                {
                    Errors = new[] {"Username already exists."}
                };
            }

            var user = new User
            {
                Guid = Guid.NewGuid(),
                UserName = username
            };
            var createdUser = await _userManager.CreateAsync(user, password);

            if (!createdUser.Succeeded)
            {
                return new AuthenticationResponse
                {
                    Errors = createdUser.Errors.Select(err => err.Description).ToArray()
                };
            }

            return new AuthenticationResponse
            {
                Success = true,
                Token = GenerateToken(user)
            };
        }

        private string GenerateToken(User user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_jwtSettings.Secret);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(JwtRegisteredClaimNames.Sub, user.UserName),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                    new Claim(JwtRegisteredClaimNames.UniqueName, user.UserName),
                    new Claim("guid", user.Guid.ToString())
                }),
                Expires = DateTime.UtcNow.AddHours(_jwtSettings.TokenExpirationHours),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
           var token = tokenHandler.CreateToken(tokenDescriptor);
           return tokenHandler.WriteToken(token);
        }
    }
}
