using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BoomaEcommerce.Domain.Policies.Operators;

namespace BoomaEcommerce.Domain.Policies
{
    public abstract class MultiPolicy : Policy
    {
        protected MultiPolicy() : base()
        {

        }
        public PolicyOperator Operator { get; set; }
        public abstract void AddPolicy(Policy policy);
        public abstract void RemovePolicy(Guid policyGuid);
    }
}
