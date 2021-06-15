using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BoomaEcommerce.Api;
using BoomaEcommerce.Api.Config;
using BoomaEcommerce.Data;
using BoomaEcommerce.Data.EfCore;
using BoomaEcommerce.Services;
using BoomaEcommerce.Tests.CoreLib;
using Microsoft.Data.SqlClient;
using Microsoft.Data.SqlClient.Server;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace BoomaEcommerce.AcceptanceTests
{
    public class SharedDatabaseFixture : IDisposable, ISharedDatabaseFixture
    {
        public TestConfig TestConfig { get; set; }
        public IServiceCollection Services { get; set; }

        public DbTransaction Transaction { get; set; }

        private object _lock = new();
        public SharedDatabaseFixture()
        {
            try
            {
                IServiceCollection serviceCollection = new ServiceCollection();

                var configuration = new ConfigurationBuilder()
                    .AddJsonFile("appsettings.Test.json", optional: true, reloadOnChange: true)
                    .Build();

                TestConfig = GetTestConfig(configuration);
                if (TestConfig.UseStubDataAccess == DbMode.InMemory) return;

                var startup = new Startup(configuration);
                startup.ConfigureServices(serviceCollection);
                serviceCollection.AddLogging();
                Services = serviceCollection;
                Connection = new SqlConnection(TestConfig.ConnectionString);
                Connection.Open();
            }
            catch
            {
                //TestConfig = new TestConfig();
            }
        }

        private TestConfig GetTestConfig(IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("TestConnectionString");
            var dbModeSection = configuration.GetSection("DbMode");
            var dataAccessMode = dbModeSection.Exists() ? dbModeSection.Get<DbMode>() : DbMode.InMemory;
            var externalSystemsSection = configuration.GetSection("UseStubExternalSystems");
            var useStubExternalSystems = !externalSystemsSection.Exists() || externalSystemsSection.Get<bool>();
            return new TestConfig
            {
                ConnectionString = connectionString,
                UseStubDataAccess = dataAccessMode,
                UseStubExternalSystems = useStubExternalSystems
            };
        }

        public DbConnection Connection { get; }

        public DbTransaction BeginTransaction()
        {
            lock (_lock)
            {
                return Connection.BeginTransaction();
            }
        }

        public ApplicationDbContext CreateContext(DbTransaction transaction = null)
        {
            var context = new ApplicationDbContext(new DbContextOptionsBuilder<ApplicationDbContext>().UseSqlServer(Connection).Options);

            if (transaction != null)
            {
                lock (_lock)
                {
                    context.Database.UseTransaction(transaction);
                }
            }

            return context;
        }

        public void Dispose() => Connection?.Dispose();

    }

    public interface ISharedDatabaseFixture
    {
    }
}
