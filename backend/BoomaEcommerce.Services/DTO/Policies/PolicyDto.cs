using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace BoomaEcommerce.Services.DTO.Policies
{
    public abstract class PolicyDto : BaseEntityDto
    {
        [JsonRequired]
        public abstract PolicyType Type { get; set; }
    }
    public class CompositePolicyDto : PolicyDto
    {
        [JsonRequired]
        public override PolicyType Type { get; set; } = PolicyType.Composite;

        [JsonRequired]
        public OperatorType? Operator { get; set; }
        public IEnumerable<PolicyDto> SubPolicies { get; set; }
    }
    public class BinaryPolicyDto : PolicyDto
    {
        [JsonRequired]
        public override PolicyType Type { get; set; } = PolicyType.Composite;

        [JsonRequired]
        public OperatorType Operator { get; set; }
        public PolicyDto FirstPolicy { get; set; }
        public PolicyDto SecondPolicy { get; set; }
    }
    public class AgeRestrictionPolicyDto : PolicyDto
    {
        [JsonRequired]
        public override PolicyType Type { get; set; } = PolicyType.AgeRestriction;

        [JsonRequired]
        public Guid ProductGuid { get; set; }

        [JsonRequired]
        public int MinAge { get; set; }
    }
    public class ProductAmountPolicyDto : PolicyDto
    {
        [JsonRequired]
        public override PolicyType Type { get; set; }

        [JsonRequired]
        public Guid ProductGuid { get; set; }

        [JsonRequired]
        public int Amount { get; set; }
    }
    public class CategoryAmountPolicyDto : PolicyDto
    {
        [JsonRequired]
        public override PolicyType Type { get; set; }

        [JsonRequired]
        public string Category { get; set; }

        [JsonRequired]
        public int Amount { get; set; }
    }
    public class TotalAmountPolicyDto : PolicyDto
    {
        [JsonRequired]
        public override PolicyType Type { get; set; }

        [JsonRequired]
        public string Category { get; set; }

        [JsonRequired]
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
        Composite,
        Binary,
        MaxTotalAmount,
        MinTotalAmount
    }
}
