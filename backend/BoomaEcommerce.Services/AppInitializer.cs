using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper.Configuration;
using BoomaEcommerce.Data;
using BoomaEcommerce.Domain;
using BoomaEcommerce.Services.Settings;
using BoomaEcommerce.Services.UseCases;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.DependencyInjection;

namespace BoomaEcommerce.Services
{
    public class AppInitializer
    {
        private readonly ILogger<AppInitializer> _logger;
        private UserManager<User> _userManager;
        private readonly IServiceProvider _sp;
        private IStoreUnitOfWork _storeUnitOfWork;
        private readonly IEnumerable<IUseCase> _useCases;
        private readonly AppInitializationSettings _settings;
        private RoleManager<IdentityRole<Guid>> _roleManager;
        public AppInitializer(ILogger<AppInitializer> logger,
            IServiceProvider sp,
            IOptions<AppInitializationSettings> options,
            IEnumerable<IUseCase> useCases)
        {
            _logger = logger;
            _sp = sp;
            _useCases = useCases;
            _settings = options.Value;
        }
        public async Task InitializeAsync()
        {
            using var scope = _sp.CreateScope();
            _storeUnitOfWork = scope.ServiceProvider.GetService<IStoreUnitOfWork>();
            _userManager = scope.ServiceProvider.GetService<UserManager<User>>();
            _roleManager = scope.ServiceProvider.GetService<RoleManager<IdentityRole<Guid>>>();
            var user = await InitAdmin();
            if (_settings.SeedDummyData)
            {
                await SeedData(user);
            }

            //await Task.WhenAll(_useCases.Where(uc => _settings.UseCases.Contains(uc.GetType().Name)).Select(uc => uc.RunUseCaseAsync()));
        }

        private async Task SeedData(User user)
        {
            _logger.LogInformation("Seeding dummy data...");
            var store = new Store
            {
                StoreFounder = user,
                StoreName = "AdminStore",
                Description = "AdminStore",
                Rating = 10
            };


            await _storeUnitOfWork.StoreRepo.InsertOneAsync(store);

            var products = Enumerable.Range(1, 10).Select(i => new Product
            {
                Store = store,
                IsSoftDeleted = false,
                Name = $"product_{i}",
                Category = "AdminProduct",
                Amount = i,
                Rating = i,
                Price = i
            }).ToList();
            await _storeUnitOfWork.ProductRepo.InsertManyAsync(products);
            var ownership = new StoreOwnership
            {
                Store = store,
                User = user
            };
            await _storeUnitOfWork.StoreRepo.InsertOneAsync(store);
            //await _storeUnitOfWork.StoreOwnershipRepo.InsertOneAsync(ownership);
            await _storeUnitOfWork.SaveAsync();
        }

        private async Task<User> InitAdmin()
        {
            _logger.LogInformation("Checking if admin exists in the system.");
            var admin = await _userManager.FindByNameAsync(_settings.AdminUserName);
            if (admin == null)
            {
                _logger.LogWarning("Admin user was not found. making attempt to insert one.");
                admin = new AdminUser
                {
                    UserName = _settings.AdminUserName
                };
                var creationResult = await _userManager.CreateAsync(admin, _settings.AdminPassword);
                if (!creationResult.Succeeded)
                {
                    _logger.LogCritical("Admin user does not exist in the system and was not able to be inserted. Exiting program.");
                    Environment.Exit(-1);
                }
            }

            _logger.LogInformation("Admin exists, checking if he has sufficient roles.");

            var roles = await _userManager.GetRolesAsync(admin);
            if (!roles.Contains(UserRoles.AdminRole))
            {
                if (_roleManager != null && await _roleManager.RoleExistsAsync(UserRoles.AdminRole))
                {
                    await _roleManager.CreateAsync(new IdentityRole<Guid>(UserRoles.AdminRole));
                }
                _logger.LogWarning("Admin role was not found for admin user, making attempt to insert the role.");
                var roleResult = await _userManager.AddToRoleAsync(admin, UserRoles.AdminRole);
                if (!roleResult.Succeeded)
                {
                    _logger.LogCritical("Failed to add role for the admin. Exiting program.");
                    Environment.Exit(-1);
                }
            }
            _logger.LogInformation("Admin with roles exists in the system.");
            return admin;
        }
    }
}
