using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BoomaEcommerce.Domain;
using BoomaEcommerce.Services.DTO;
using FluentValidation;

namespace BoomaEcommerce.Services.Purchases
{
    public static class PurchaseServiceValidators
    {
        public class CreatePurchaseAsync : AbstractValidator<PurchaseDto>
        {
            public CreatePurchaseAsync()
            {
                RuleFor(p => p.Guid == default);
                RuleFor(p => p.TotalPrice).GreaterThanOrEqualTo(0);
                RuleFor(p => p.StorePurchases).NotNull();
                RuleForEach(p => p.StorePurchases).SetValidator(new StorePurchaseValidator());
            }
        }
        
        private class StorePurchaseValidator : AbstractValidator<StorePurchaseDto>
        {
            public StorePurchaseValidator()
            {
                RuleFor(p => p.Guid == default);
                RuleFor(p => p.TotalPrice).GreaterThanOrEqualTo(0);
                RuleFor(p => p.Store)
                    .NotNull()
                    .Must(store => store.Guid != default);
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
                RuleFor(pp => pp.Product)
                    .NotNull()
                    .Must(p => p.Guid != default);
            }    
        }
    }
}
