using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using BoomaEcommerce.Data;
using BoomaEcommerce.Domain;
using BoomaEcommerce.Services.DTO;
using BoomaEcommerce.Services.Settings;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;


namespace BoomaEcommerce.Services.Authentication
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly ILogger<AuthenticationService> _logger;
        private readonly UserManager<User> _userManager;
        private readonly JwtSettings _jwtSettings;
        private readonly TokenValidationParameters _tokenValidationParams;
        private readonly IRepository<RefreshToken> _refreshTokenRepo;
        private readonly IMapper _mapper;

        public AuthenticationService(
            ILogger<AuthenticationService> logger,
            UserManager<User> userManager,
            JwtSettings jwtSettings,
            TokenValidationParameters tokenValidationParams, 
            IRepository<RefreshToken> refreshTokenRepo, IMapper mapper)
        {
            _logger = logger;
            _userManager = userManager;
            _jwtSettings = jwtSettings;
            _tokenValidationParams = tokenValidationParams;
            _refreshTokenRepo = refreshTokenRepo;
            _mapper = mapper;
        }
        public AuthenticationService(ILogger<AuthenticationService> logger,
            UserManager<User> userManager,
            IOptions<JwtSettings> jwtOptions, TokenValidationParameters tokenValidationParams,
            IRepository<RefreshToken> refreshTokenRepo, IMapper mapper) 
            : this(logger, userManager, jwtOptions.Value, tokenValidationParams, refreshTokenRepo, mapper) { }

        public async Task<AuthenticationResult> LoginAsync(string username, string password)
        {
            _logger.LogInformation($"Making attempt to log in for user: {username}");

            var existingUser = await _userManager.FindByNameAsync(username);

            if (existingUser == null)
            {
                _logger.LogWarning($"User with username {username} does not exist in the system.");
                return new AuthenticationResult 
                {
                    Errors = new[] { "Username doesn't exist." }
                };
            }

            var userHasValidPassword = await _userManager.CheckPasswordAsync(existingUser, password);

            if (!userHasValidPassword)
            {
                _logger.LogWarning($"Password for user with username {username} is incorrect.");
                return new AuthenticationResult
                {
                    Errors = new[] { "Bad username or password." }
                };
            }

            var roles = await _userManager.GetRolesAsync(existingUser);

            return await GenerateAuthResponseWithToken(existingUser, roles.ToArray());
        }

        public async Task<AuthenticationResult> RegisterAdminAsync(AdminUserDto userDto, string password)
        {
            _logger.LogInformation($"Making attempt to register admin: {userDto.UserName}");
            var existingUser = await _userManager.FindByNameAsync(userDto.UserName);

            if (existingUser != null)
            {
                _logger.LogInformation($"Admin user with username {userDto.UserName} already exist in the system.");

                return new AuthenticationResult
                {
                    Errors = new[] { "Username already exists." }
                };
            }

            var user = _mapper.Map<AdminUser>(userDto);
            var createdUser = await _userManager.CreateAsync(user, password);

            if (!createdUser.Succeeded)
            {
                _logger.LogWarning($"Failed to register admin user with username {user.UserName}.");
                return new AuthenticationResult
                {
                    Errors = createdUser.Errors.Select(err => err.Description).ToArray()
                };
            }
            
            var roleResult = await _userManager.AddToRoleAsync(user, "Admin");
            if (roleResult == null || !roleResult.Succeeded)
            {
                _logger.LogWarning($"Failed to add admin role to user with username {user.UserName}.");
                return new AuthenticationResult
                {
                    Errors = createdUser.Errors.Select(err => err.Description).ToArray()
                };
            }

            return await GenerateAuthResponseWithToken(user, UserRoles.AdminRole);
        }

        public async Task<AuthenticationResult> RegisterAsync(UserDto userDto, string password)
        {
            _logger.LogInformation($"Making attempt to register user: {userDto.UserName}");
            var existingUser = await _userManager.FindByNameAsync(userDto.UserName);

            if (existingUser != null)
            {
                _logger.LogInformation($"User with username {userDto.UserName} already exist in the system.");

                return new AuthenticationResult
                {
                    Errors = new[] {"Username already exists."}
                };
            }

            var user = _mapper.Map<User>(userDto);
            var createdUser = await _userManager.CreateAsync(user, password);

            if (!createdUser.Succeeded)
            {
                _logger.LogWarning($"Failed to register user with username {userDto.UserName}.");
                return new AuthenticationResult
                {
                    Errors = createdUser.Errors.Select(err => err.Description).ToArray()
                };
            }
            _logger.LogInformation("Successfully registered user: {user}", user.UserName);
            return await GenerateAuthResponseWithToken(user);
        }

        public async Task<AuthenticationResult> RefreshJwtToken(string token, string refreshToken)
        {
            var validatedToken = GetClaimsFromToken(token);
            var expirationClaim = validatedToken?.Claims.SingleOrDefault(x => x.Type == JwtRegisteredClaimNames.Exp)?.Value;
            if (validatedToken == null || expirationClaim == null)
            {
                _logger.LogWarning("Token: {token} is invalid.");
                return new AuthenticationResult()
                {
                    Errors = new[] {"Invalid token"}
                };
            }

            var expirationDateUnix = long.Parse(expirationClaim);
            var expirationDateTimeUtc = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)
                .AddSeconds(expirationDateUnix)
                .Subtract(_jwtSettings.TokenLifeTime);

            if (expirationDateTimeUtc > DateTime.UtcNow)
            {
                return new AuthenticationResult
                {
                    Errors = new[] {"This token has not expired yet."}
                };
            }

            var storedRefreshToken = await _refreshTokenRepo.FindOneAsync(rfToken => rfToken.Token == refreshToken);

            if (DateTime.UtcNow > storedRefreshToken.ExpirationDate)
            {
                return new AuthenticationResult
                {
                    Errors = new[] {"This refresh token has expired."}
                };
            }

            var jti = validatedToken.Claims.SingleOrDefault(claim => claim.Type == JwtRegisteredClaimNames.Jti)?.Value;

            if (storedRefreshToken.Used)
            {
                return new AuthenticationResult {Errors = new[] {" This refresh token has been used."}};
            }

            if (storedRefreshToken.JwtId != jti)
            {
                return new AuthenticationResult {Errors = new[] {"This refresh token does not exist."}};
            }

            storedRefreshToken.Used = true;
            await _refreshTokenRepo.ReplaceOneAsync(storedRefreshToken);
            
            var userGuid = validatedToken.Claims.Single(claim => claim.Type == "guid").Value;
            var user = await _userManager.FindByIdAsync(userGuid);
            var isAdmin = await _userManager.IsInRoleAsync(user, UserRoles.AdminRole);

            return await (isAdmin
                ? GenerateAuthResponseWithToken(user, UserRoles.AdminRole)
                : GenerateAuthResponseWithToken(user));

        }

        private bool IsJwtWithValidSecurityAlgorithm(SecurityToken validationToken) =>
            validationToken is JwtSecurityToken jwtSecurityToken &&
            jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256,
                StringComparison.InvariantCultureIgnoreCase);

        private ClaimsPrincipal GetClaimsFromToken(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            try
            {
                var claimsPrincipal = tokenHandler.ValidateToken(token, _tokenValidationParams, out var validatedToken);
                return !IsJwtWithValidSecurityAlgorithm(validatedToken) 
                    ? null 
                    : claimsPrincipal;
            }
            catch(Exception e)
            {
                _logger.LogError(e, "Failed to read claims from token {token}", token);
                return null;
            }
        }

        private async Task<AuthenticationResult> GenerateAuthResponseWithToken(User user, params string[] roles)
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
                }.Concat(roles.Select(role => new Claim(ClaimTypes.Role, role)))),
                Expires = DateTime.UtcNow.Add(_jwtSettings.TokenLifeTime),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);

            var refreshToken = new RefreshToken
            {
                JwtId = token.Id,
                User = user,
                CreationDate = DateTime.UtcNow,
                ExpirationDate = DateTime.UtcNow.AddMonths(_jwtSettings.RefreshTokenExpirationMonthsAmount),
                Token = Guid.NewGuid().ToString()
            };

            try
            {
                await _refreshTokenRepo.InsertOneAsync(refreshToken);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Failed to get refresh {refreshToken}", refreshToken);
            }

            var tokenString = tokenHandler.WriteToken(token);
            return new AuthenticationResult
            {
                Success = true,
                Token = tokenString,
                RefreshToken = refreshToken.Token,
                UserGuid = user.Guid
            };
        }
    }
}
