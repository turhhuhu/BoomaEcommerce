using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BoomaEcommerce.Services.DTO;
using FluentValidation;

namespace BoomaEcommerce.Services.Stores
{
    public static class StoreServiceValidators
    {
        public class CreateStoreProductValidator : AbstractValidator<ProductDto>
        {
            public CreateStoreProductValidator()
            {
                RuleFor(product => product.Guid)
                    .Must(guid => guid == default);

                RuleFor(product => product.Amount)
                    .GreaterThan(0);

                RuleFor(product => product.Store)
                    .NotNull()
                    .Must(store => store.Guid != default);

                RuleFor(product => product.Price)
                    .GreaterThan(0);

                RuleFor(product => product.Rating)
                    .Null();
            }
        }
        
        public class CreateStoreAsync : AbstractValidator<StoreDto>
        {
            public CreateStoreAsync()
            {
                RuleFor(store => store.Guid)
                    .Must(guid => guid == default);
                RuleFor(store => store.Rating)
                    .Null();
                RuleFor(store => store.StoreFounder)
                    .Must(founder => founder.Guid != default);
            }
        }

        public class UpdateProductAsync : AbstractValidator<ProductDto>
        {
            public UpdateProductAsync()
            {
                RuleFor(product => product.Guid)
                    .Must(guid => guid != default);

                RuleFor(product => product.Amount)
                    .GreaterThan(0);

                RuleFor(product => product.Store)
                    .NotNull()
                    .Must(store => store.Guid != default);

                RuleFor(product => product.Price)
                    .GreaterThan(0);
            }
        }

        public class NominateNewStoreManager : AbstractValidator<StoreManagementDto>
        {
            public NominateNewStoreManager()
            {
                RuleFor(sm => sm.Guid)
                    .Must(guid => guid == default);
                RuleFor(sm => sm.Permissions)
                    .NotNull();
                RuleFor(sm => sm.Store)
                    .NotNull()
                    .Must(store => store.Guid != default);
                RuleFor(sm => sm.User)
                    .NotNull()
                    .Must(user => user.Guid != default);
            }
        }
    }
}
