using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Newtonsoft.Json.Converters;

namespace BoomaEcommerce.Services.DTO.Policies
{
    public abstract class PolicyDto : BaseEntityDto
    {
        public abstract PolicyType Type { get; set; }
    }
    public class CompositePolicyDto : PolicyDto
    {
        public override PolicyType Type { get; set; } = PolicyType.Composite;
        public OperatorType Operator { get; set; }
        public IEnumerable<PolicyDto> SubPolicies { get; set; }
    }
    public class AgeRestrictionPolicyDto : PolicyDto
    {
        public override PolicyType Type { get; set; } = PolicyType.AgeRestriction;
        public Guid ProductGuid { get; set; }
        public int MinAge { get; set; }
    }
    public class ProductAmountPolicyDto : PolicyDto
    {
        public override PolicyType Type { get; set; }
        public Guid ProductGuid { get; set; }
        public int Amount { get; set; }
    }
    public class CategoryAmountPolicyDto : PolicyDto
    {
        public override PolicyType Type { get; set; }
        public string Category { get; set; }
        public int Amount { get; set; }
    }
    public enum OperatorType
    {
        And,
        Or,
        Condition,
        Xor
    }

    public enum PolicyType
    {
        AgeRestriction,
        MaxCategoryAmount,
        MinCategoryAmount,
        MaxProductAmount,
        MinProductAmount,
        Composite
    }
}
