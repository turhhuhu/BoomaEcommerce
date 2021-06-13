using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BoomaEcommerce.Domain;
using BoomaEcommerce.Services.DTO;
using FluentValidation;
using FluentValidation.Validators;

namespace BoomaEcommerce.Services.Purchases
{
    public static class PurchaseServiceValidators
    {
        public class CreatePurchaseAsync : AbstractValidator<PurchaseDetailsDto>
        {
            public CreatePurchaseAsync()
            {
                RuleFor(p => p.Purchase).SetValidator(new PurchaseValidator());
                RuleFor(p => p.PaymentDetails).SetValidator(new PaymentDetailsValidator());
                RuleFor(p => p.SupplyDetails).SetValidator(new SupplyDetailsValidator());
            }
        }

        public class PurchaseValidator : AbstractValidator<PurchaseDto>
        {
            public PurchaseValidator()
            {
                RuleFor(p => p.Guid == default);
                RuleFor(p => p.TotalPrice).GreaterThanOrEqualTo(0);
                RuleFor(p => p.StorePurchases).NotNull();
                RuleFor(p => p.UserBuyerGuid)
                    .NotNull()
                    .When(p => p.Buyer == null);
                RuleFor(p => p.Buyer)
                    .SetValidator(new GuestBasicInfoValidator())
                    .When(p => !p.UserBuyerGuid.HasValue);
                RuleForEach(p => p.StorePurchases).SetValidator(new StorePurchaseValidator());
            }
        }

        private class GuestBasicInfoValidator : AbstractValidator<BasicUserInfoDto>
        {
            public GuestBasicInfoValidator()
            {
                RuleFor(g => g.LastName)
                    .NotEmpty();
                RuleFor(g => g.Name)
                    .NotEmpty();
                RuleFor(g => g.Guid)
                    .Must(guid => guid == default);
            }
        }
        private class StorePurchaseValidator : AbstractValidator<StorePurchaseDto>
        {
            public StorePurchaseValidator()
            {
                RuleFor(p => p.Guid == default);
                RuleFor(p => p.TotalPrice).GreaterThanOrEqualTo(0);
                RuleFor(p => p.StoreGuid)
                    .NotNull()
                    .Must(store => store != default);
                RuleFor(p => p.PurchaseProducts).NotNull();
                RuleForEach(p => p.PurchaseProducts).SetValidator(new PurchaseProductValidator());
            }
        }
        
        private class PurchaseProductValidator : AbstractValidator<PurchaseProductDto>
        {
            public PurchaseProductValidator()
            {
                RuleFor(pp => pp.Guid == default);
                RuleFor(pp => pp.Price).GreaterThanOrEqualTo(0);
                RuleFor(pp => pp.Amount).GreaterThan(0);
                RuleFor(pp => pp.ProductGuid)
                    .Must(p => p != default);
            }    
        }
        private class SupplyDetailsValidator : AbstractValidator<SupplyDetailsDto>
        {
            public SupplyDetailsValidator()
            {
                RuleFor(s => s.Address).NotEmpty();
                RuleFor(s => s.City).NotEmpty();
                RuleFor(s => s.Name).NotEmpty();
                RuleFor(s => s.Zip)
                    .NotEmpty()
                    .WithMessage("Zip code must be not empty all numbers.");
            }
        }

        private class PaymentDetailsValidator : AbstractValidator<PaymentDetailsDto>
        {
            public PaymentDetailsValidator()
            {
                RuleFor(p => p.CardNumber)
                    .NotEmpty();

                RuleFor(p => p.HolderName)
                    .NotEmpty()
                    .WithMessage("Card holder can not be empty.");

            }
        }
    }


}
