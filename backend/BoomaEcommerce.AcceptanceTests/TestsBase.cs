using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using BoomaEcommerce.Api.Config;
using BoomaEcommerce.Data;
using BoomaEcommerce.Data.EfCore;
using BoomaEcommerce.Domain;
using BoomaEcommerce.Services;
using BoomaEcommerce.Tests.CoreLib;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace BoomaEcommerce.AcceptanceTests
{
    public abstract class TestsBase : IAsyncLifetime, IClassFixture<SharedDatabaseFixture>
    {
        private static readonly SemaphoreSlim Semaphore = new(1);
        public SharedDatabaseFixture DataBaseFixture { get; }
        private DbTransaction _transaction;
        private ApplicationDbContext _dbContext;
        protected TestsBase(SharedDatabaseFixture dataBaseFixture)
        {
            DataBaseFixture = dataBaseFixture;
        }
        public virtual async Task InitializeAsync()
        {
            if (DataBaseFixture.TestConfig.UseStubDataAccess == DbMode.InMemory)
            {
                await InitInMemoryDb();
            }
            else
            {
                try
                {
                    await Semaphore.WaitAsync();
                    DataBaseFixture.Services.AddTransient<IPurchaseUnitOfWork, PurchaseUnitOfWorkStub>();
                    _transaction = DataBaseFixture.Connection.BeginTransaction();
                    _dbContext = DataBaseFixture.CreateContext(_transaction);
                    DataBaseFixture.Services.AddTransient(_ => _dbContext);
                    var notfPublisher = new NotificationPublisherStub();
                    DataBaseFixture.Services.AddSingleton<INotificationPublisher, NotificationPublisherStub>(_ => notfPublisher);
                    DataBaseFixture.Services.AddSingleton(_ => notfPublisher);

                    var provider = DataBaseFixture.Services.BuildServiceProvider();
                    var roleManager = provider.GetRequiredService<RoleManager<IdentityRole<Guid>>>();
                    if (!await roleManager.RoleExistsAsync(UserRoles.AdminRole))
                    {
                        await roleManager.CreateAsync(new IdentityRole<Guid>(UserRoles.AdminRole));
                    }
                    await InitEfCoreDb(provider);
                }
                catch
                {
                    Semaphore.Release();
                }
            }
        }

        public async Task DisposeAsync()
        {
            if (_transaction != null)
            {
                await _transaction.DisposeAsync();
            }
            if (_dbContext != null)
            {
                await _dbContext.DisposeAsync();
            }

            Semaphore.Release();
        }

        public virtual Task InitInMemoryDb()
        {
            if (!DataBaseFixture.TestConfig.UseStubExternalSystems)
            {
                throw new Exception("InMemory db does not support using non-stub external systems.");
            }
            return Task.CompletedTask;
        }
        public abstract Task InitEfCoreDb(ServiceProvider provider);
    }
}
