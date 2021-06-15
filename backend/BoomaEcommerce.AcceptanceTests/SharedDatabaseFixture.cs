using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BoomaEcommerce.Data.EfCore;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace BoomaEcommerce.AcceptanceTests
{
    public class SharedDatabaseFixture : IDisposable, ISharedDatabaseFixture
    {
        public TestConfig TestConfig { get; set; }
        public SharedDatabaseFixture()
        {
            try
            {
                var config = new ConfigurationBuilder()
                    .AddJsonFile("appsettings.Test.json", optional: true)
                    .Build();
                TestConfig = config.GetSection("TestConfig").Get<TestConfig>();
                if (TestConfig.UseStubDataAccess) return;
                Connection = new SqlConnection(TestConfig.ConnectionString);
                Connection.Open();
            }
            catch
            {
                // ignored
            }
        }

        public DbConnection Connection { get; }

        public ApplicationDbContext CreateContext(DbTransaction transaction = null)
        {
            var context = new ApplicationDbContext(new DbContextOptionsBuilder<ApplicationDbContext>().UseSqlServer(Connection).Options);

            if (transaction != null)
            {
                context.Database.UseTransaction(transaction);
            }

            return context;
        }

        public void Dispose() => Connection.Dispose();

    }

    public interface ISharedDatabaseFixture
    {
    }
}
