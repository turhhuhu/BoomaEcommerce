using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BoomaEcommerce.AcceptanceTests
{
    public class TestConfig
    {
        public bool UseStubExternalSystems { get; set; } = true;
        public bool UseStubDataAccess { get; set; } = true;
        public string ConnectionString { get; set; }
    }
}
