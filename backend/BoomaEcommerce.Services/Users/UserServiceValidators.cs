using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BoomaEcommerce.Services.DTO;
using BoomaEcommerce.Services.DTO.ProductOffer;
using FluentValidation;

namespace BoomaEcommerce.Services.Users
{
    public static class UserServiceValidators
    {
        public class CreateShoppingBasketAsync : AbstractValidator<ShoppingBasketDto>
        {
            public CreateShoppingBasketAsync()
            {
                RuleFor(sb => sb.Guid == default);
                RuleFor(sb => sb.StoreGuid)
                    .Must(store => store != default);
                RuleFor(sb => sb.PurchaseProducts)
                    .NotEmpty();
            }
        }

        public class AddPurchaseProductToShoppingBasketAsync : AbstractValidator<PurchaseProductDto>
        {
            public AddPurchaseProductToShoppingBasketAsync()
            {
                RuleFor(pp => pp.Guid == default);
                RuleFor(pp => pp.Amount).GreaterThan(0);
                RuleFor(pp => pp.Price).GreaterThanOrEqualTo(0);
                RuleFor(pp => pp.ProductGuid)
                    .Must(p => p != default);
            }
        }

        public class UpdateUserInfoAsync : AbstractValidator<UserDto>
        {
            public UpdateUserInfoAsync()
            {
                RuleFor(u => u.Guid != default);
            }
        }

        internal class CreateProductOffer : AbstractValidator<ProductOfferDto>
        {
            public CreateProductOffer()
            {
                RuleFor(offer => offer.OfferPrice).GreaterThanOrEqualTo(0);
                RuleFor(offer => offer.State).Equal(ProductOfferStateDto.Pending);
                RuleFor(offer => offer.Product.Guid != default);
            }
        }
    }
}
