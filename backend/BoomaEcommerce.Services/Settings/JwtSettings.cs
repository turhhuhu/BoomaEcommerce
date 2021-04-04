﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BoomaEcommerce.Services.Settings
{
    public class JwtSettings
    {
        public string Secret { get; set; }
        public double TokenExpirationHours { get; set; }
    }
}