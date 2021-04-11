﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BoomaEcommerce.Core;

namespace BoomaEcommerce.Domain
{
    public class StoreManagementPermission : BaseEntity
    {
        // Example flag of permission.
        public bool CanDoSomething { get; set; }
        public bool ExampleFlag1 { get; set; }
        public bool ExampleFlag2 { get; set; }
        public StoreManagement StoreManagement { get; set; }
    }
}
