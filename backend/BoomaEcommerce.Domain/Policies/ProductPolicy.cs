using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BoomaEcommerce.Domain.Policies
{
    public abstract class ProductPolicy : Policy
    {
        public Product Product { get; set; }

        protected ProductPolicy(Product product)
        {
            Product = product;
        }

        protected ProductPolicy()
        {
            
        }
    }
}
