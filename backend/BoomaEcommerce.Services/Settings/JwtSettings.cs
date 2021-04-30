using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BoomaEcommerce.Services.Settings
{
    public class JwtSettings
    {
        public const string Section = "Jwt";
        public string Secret { get; set; }
        public TimeSpan TokenLifeTime { get; set; }
        public int RefreshTokenExpirationMonthsAmount { get; set; }
    }
}
