using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BoomaEcommerce.Services.Settings
{
    public class AppInitializationSettings
    {
        public const string Section = "AppInitialization";
        public bool SeedDummyData { get; set; }
        public List<string> UseCases { get; set; }
        public string AdminUserName { get; set; } = "Admin";
        public string AdminPassword { get; set; } = "Admin";
    }
}
