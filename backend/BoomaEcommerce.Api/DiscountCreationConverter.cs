using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BoomaEcommerce.Services.DTO.Discounts;
using BoomaEcommerce.Services.DTO.Policies;
using FluentValidation;
using FluentValidation.Results;
using Newtonsoft.Json.Linq;

namespace BoomaEcommerce.Api
{
    public class DiscountCreationConverter : JsonCreationConverter<DiscountDto>
    {
        protected override DiscountDto Create(Type objectType, JObject jObject)
        {
            if (!jObject.ContainsKey("type"))
            {
                throw new ValidationException(new[] { new ValidationFailure(nameof(DiscountDto.Type), "Type of Discount must be provided.") });
            }
            var type = jObject["type"].ToObject<DiscountType>();
            return type switch
            {
                DiscountType.Category => new CategoryDiscountDto(),
                DiscountType.Product => new ProductDiscountDto(),
                DiscountType.Basket => new BasketDiscountDto(),
                DiscountType.Composite => new CompositeDiscountDto(),
                _ => throw new ArgumentOutOfRangeException()
            };
        }
    }
}