using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper.Configuration;
using BoomaEcommerce.Data;
using BoomaEcommerce.Domain;
using BoomaEcommerce.Services.Settings;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace BoomaEcommerce.Services
{
    public class AppInitializer
    {
        private readonly ILogger<AppInitializer> _logger;
        private readonly UserManager<User> _userManager;
        private readonly IStoreUnitOfWork _storeUnitOfWork;
        private readonly AppInitializationSettings _settings;
        public AppInitializer(ILogger<AppInitializer> logger,
            UserManager<User> userManager,
            IStoreUnitOfWork storeUnitOfWork,
            IOptions<AppInitializationSettings> options)
        {
            _logger = logger;
            _userManager = userManager;
            _storeUnitOfWork = storeUnitOfWork;
            _settings = options.Value;
        }
        public async Task InitializeAsync()
        {
            var user = await InitAdmin();
            if (_settings.SeedDummyData)
            {
                await SeedData(user);
            }
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
            await _storeUnitOfWork.StoreOwnershipRepo.InsertOneAsync(ownership);
        }

        private async Task<User> InitAdmin()
        {
            _logger.LogInformation("Checking if admin exists in the system.");
            var admin = await _userManager.FindByNameAsync(UserRoles.AdminRole);
            if (admin == null)
            {
                _logger.LogWarning("Admin user was not found. making attempt to insert one.");
                admin = new AdminUser
                {
                    UserName = UserRoles.AdminRole,
                    Name = UserRoles.AdminRole,
                    LastName = UserRoles.AdminRole
                };
                var creationResult = await _userManager.CreateAsync(admin, UserRoles.AdminRole);
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
