using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using BoomaEcommerce.Services.Authentication;
using BoomaEcommerce.Services.DTO;
using BoomaEcommerce.Services.Settings;
using BoomaEcommerce.Services.Stores;
using BoomaEcommerce.Services.Users;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.DependencyInjection;

namespace BoomaEcommerce.Services.UseCases
{
    public class StoreFounderActionsUseCase : IUseCase
    {
        private readonly IHttpContextAccessor _accessor;
        private readonly IAuthenticationService _authService;
        private readonly IServiceProvider _sp;
        private readonly JwtSettings _jwtSettings;

        public StoreFounderActionsUseCase(
            IHttpContextAccessor accessor,
            IAuthenticationService authService,
            IServiceProvider sp,
            IOptions<JwtSettings> jwtSettings)
        {
            _accessor = accessor;
            _authService = authService;
            _sp = sp;
            _jwtSettings = jwtSettings.Value;
        }
        public async Task RunUseCaseAsync()
        {

            await RegisterUsers();

            var loginRes = await _authService.LoginAsync("u2", "u2pass");

            var claims = SecuredServiceBase.ValidateToken(loginRes.Token, _jwtSettings.Secret);

            _accessor.HttpContext = new DefaultHttpContext
            {
                User = claims
            };

            var storeService = _sp.GetRequiredService<IStoresService>();
            var userService = _sp.GetRequiredService<IUsersService>();

            var createdStore = await storeService.CreateStoreAsync(new StoreDto
            {
                StoreName = "s1",
                Description = "u2 store"
            });

            await storeService.CreateStoreProductAsync(new ProductDto
            {
                StoreGuid = createdStore.Guid,
                Amount = 20,
                Category = "snacks",
                Name = "Bamba",
                Price = 30
            });

            var u3 = await userService.GetBasicUserInfoAsync("u3");
            var u2Ownership = await storeService.GetStoreOwnerShipAsync(loginRes.UserGuid, createdStore.Guid);

            await storeService.NominateNewStoreManagerAsync(u2Ownership.Guid, new StoreManagementDto
            {
                Store = new StoreDto
                {
                    Guid = createdStore.Guid
                },
                User = new UserDto
                {
                    Guid = u3.Guid
                },
                Permissions = new StoreManagementPermissionsDto
                {
                    CanAddProduct = true,
                    CanDeleteProduct = true,
                    CanUpdateProduct = true
                }
            });
        }

        private async Task RegisterUsers()
        {
            var userU1 = new AdminUserDto
            {
                UserName = "u1",
                Name = "u1",
                LastName = "u1"
            };

            await _authService.RegisterAsync(userU1, "u1pass");

            var userU2 = new UserDto
            {
                UserName = "u2",
                Name = "u2",
                LastName = "u2"
            };
            await _authService.RegisterAsync(userU2, "u2pass");

            var userU3 = new UserDto
            {
                UserName = "u3",
                Name = "u3",
                LastName = "u3"
            };
            await _authService.RegisterAsync(userU3, "u3pass");
        }
    }
}
