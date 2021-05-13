using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BoomaEcommerce.Core;

namespace BoomaEcommerce.Domain
{
    public class Store : BaseEntity
    {
        private readonly IStorePolicy _defaultPolicy = null;
        public string StoreName { get; set; }
        public string Description { get; set; }
        public User StoreFounder { get; set; }
        public float Rating { get; set; }
        public IStorePolicy StorePolicy { get; set; }

        public Store(IStorePolicy storePolicy)
        {
            this.StorePolicy = storePolicy;
        }

        public Store()
        {
            this.StorePolicy = _defaultPolicy;
        }
        
    }

}
