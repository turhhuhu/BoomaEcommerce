using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BoomaEcommerce.Domain;
using BoomaEcommerce.Services.DTO;
using FluentValidation;

namespace BoomaEcommerce.Services.Stores
{
    public static class StoreServiceValidators
    {
        public class CreateStoreProduct : AbstractValidator<ProductDto>
        {
            public CreateStoreProduct()
            {
                RuleFor(product => product.Guid)
                    .Must(guid => guid == default);

                RuleFor(product => product.Amount)
                    .GreaterThan(0);

                RuleFor(product => product.Store)
                    .Must(store => store != null && store.Guid != default);


                RuleFor(product => product.Price)
                    .GreaterThan(0);

                RuleFor(product => product.Rating)
                    .Null();
            }
        }

        public class UpdateStoreProduct : AbstractValidator<ProductDto>
        {
            public UpdateStoreProduct()
            {
                RuleFor(product => product.Guid)
                    .Must(guid => guid != default);

                RuleFor(product => product.Amount)
                    .GreaterThan(0)
                    .When(product => product.Amount.HasValue);

                RuleFor(product => product.Store)
                    .Must(store => store != null && store.Guid != default);

                RuleFor(product => product.Price)
                    .GreaterThan(0)
                    .When(product => product.Price.HasValue);

                RuleFor(product => product.Rating)
                    .Null();
            }
        }
        public class CreateStore : AbstractValidator<StoreDto>
        {
            public CreateStore()
            {
                RuleFor(store => store.Guid)
                    .Must(guid => guid == default);
                RuleFor(store => store.Rating)
                    .Null();
                RuleFor(store => store.StoreFounder)
                    .Must(founder => founder.Guid != default);
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
                    .Must(store => store != null && store.Guid != default);
                RuleFor(sm => sm.User)
                    .Must(user => user != null && user.Guid != default);
            }
        }
        public class NominateNewStoreOwner : AbstractValidator<StoreOwnershipDto>
        {
            public NominateNewStoreOwner()
            {
                RuleFor(sm => sm.Guid)
                    .Must(guid => guid == default);
                RuleFor(sm => sm.Store)
                    .Must(store => store != null && store.Guid != default);
                RuleFor(sm => sm.User)
                    .Must(user => user != null && user.Guid != default);
            }
        }

        public class UpdateManagerPermission : AbstractValidator<StoreManagementPermissionDto>
        {
            public UpdateManagerPermission()
            {
                RuleFor(permissions => permissions.Guid)
                    .Must(guid => guid != default);
            }
        }
    }
}
