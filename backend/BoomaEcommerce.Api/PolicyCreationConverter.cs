using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BoomaEcommerce.Services.DTO.Policies;
using Newtonsoft.Json.Linq;

namespace BoomaEcommerce.Api
{
    public class PolicyCreationConverter : JsonCreationConverter<PolicyDto>
    {
        protected override PolicyDto Create(Type objectType, JObject jObject)
        {
            var type = jObject["type"].ToObject<PolicyType>();
            return type switch
            {
                PolicyType.AgeRestriction => new AgeRestrictionPolicyDto(),
                PolicyType.MaxCategoryAmount => new CategoryAmountPolicyDto(),
                PolicyType.MinCategoryAmount => new CategoryAmountPolicyDto(),
                PolicyType.MaxProductAmount => new ProductAmountPolicyDto(),
                PolicyType.MinProductAmount => new ProductAmountPolicyDto(),
                PolicyType.Composite => new CompositePolicyDto(),
                _ => throw new ArgumentOutOfRangeException()
            };
        }
    }
}
