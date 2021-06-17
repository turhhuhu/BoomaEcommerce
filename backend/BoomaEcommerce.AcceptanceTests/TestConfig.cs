using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BoomaEcommerce.Api.Config;

namespace BoomaEcommerce.AcceptanceTests
{
    public class TestConfig
    {
        public bool UseStubExternalSystems { get; set; } = true;
        public DbMode UseStubDataAccess { get; set; } = DbMode.InMemory;
        public string ConnectionString { get; set; }
    }
}
