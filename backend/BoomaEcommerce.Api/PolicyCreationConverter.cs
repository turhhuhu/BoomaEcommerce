using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BoomaEcommerce.Services.DTO.Policies;
using FluentValidation;
using FluentValidation.Results;
using Newtonsoft.Json.Linq;

namespace BoomaEcommerce.Api
{
    public class PolicyCreationConverter : JsonCreationConverter<PolicyDto>
    {
        protected override PolicyDto Create(Type objectType, JObject jObject)
        {
            if (!jObject.ContainsKey("type"))
            {
                throw new ValidationException(new []{new ValidationFailure(nameof(PolicyDto.Type), "Type of policy must be provided.") });
            }
            var type = jObject["type"].ToObject<PolicyType>();
            return type switch
            {
                PolicyType.AgeRestriction => new AgeRestrictionPolicyDto(),
                PolicyType.MaxCategoryAmount => new CategoryAmountPolicyDto(),
                PolicyType.MinCategoryAmount => new CategoryAmountPolicyDto(),
                PolicyType.MaxProductAmount => new ProductAmountPolicyDto(),
                PolicyType.MinProductAmount => new ProductAmountPolicyDto(),
                PolicyType.MaxTotalAmount => new TotalAmountPolicyDto(),
                PolicyType.MinTotalAmount => new TotalAmountPolicyDto(),
                PolicyType.Composite => new CompositePolicyDto(),
                PolicyType.Binary => new BinaryPolicyDto(),
                _ => throw new ArgumentOutOfRangeException()
            };
        }
    }
}
