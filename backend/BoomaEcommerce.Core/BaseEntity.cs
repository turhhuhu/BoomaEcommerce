﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BoomaEcommerce.Core
{
    public class BaseEntity
    {
        public Guid Guid { get; set; }

        public BaseEntity(Guid guid)
        {
            Guid = guid;
        }

        public BaseEntity()
        {
            Guid = Guid.NewGuid();
        }
    }
}
