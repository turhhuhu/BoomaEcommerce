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
                DiscountType.CategoryDiscount => new CategoryDiscountDto(),
                DiscountType.ProductDiscount => new ProductDiscountDto(),
                DiscountType.BasketDiscount => new BasketDiscountDto(),
                DiscountType.CompositeDiscount => new CompositeDiscountDto(),
                _ => throw new ArgumentOutOfRangeException()
            };
        }
    }
}