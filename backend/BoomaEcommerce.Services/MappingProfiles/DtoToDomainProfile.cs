using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using AutoMapper;
using BoomaEcommerce.Core;
using BoomaEcommerce.Domain;
using BoomaEcommerce.Domain.Discounts;
using BoomaEcommerce.Domain.Discounts.Operators;
using BoomaEcommerce.Domain.Policies;
using BoomaEcommerce.Domain.Policies.Operators;
using BoomaEcommerce.Domain.Policies.PolicyTypes;
using BoomaEcommerce.Services.DTO;
using BoomaEcommerce.Services.DTO.Discounts;
using BoomaEcommerce.Services.DTO.Policies;

namespace BoomaEcommerce.Services.MappingProfiles
{
    /// <summary>
    /// A class that holds all the mappings from DTO objects to Domain objects.
    /// </summary>
    public class DtoToDomainProfile : Profile
    {
        public DtoToDomainProfile()
        {
            CreateMap<UserDto, User>()
                .IncludeAllDerived()
                .ForMember(x => x.Guid, x => x.Condition(xx => xx.Guid != default));

            CreateMap<StoreDto, Store>()
                .ForMember(store => store.StoreFounder, x => x.MapFrom(dto => new User {Guid = dto.FounderUserGuid}))
                .ForMember(store => store.StoreDiscount, x => x.MapFrom(_ => Discount.Empty))
                .ForMember(store => store.StorePolicy, x => x.MapFrom(_ => Policy.Empty));

            CreateMap<List<PurchaseProductDto>, ISet<PurchaseProduct>>()
                .ConstructUsing((x, y) => x.Select(pp => y.Mapper.Map<PurchaseProduct>(pp)).ToHashSet(new EqualityComparers.SameGuid<PurchaseProduct>()));

            CreateMap<ShoppingBasketDto, ShoppingBasket>()
                .ForMember(basket => basket.Store, x => x.MapFrom(dto => new Store {Guid = dto.StoreGuid}))
                .ForMember(basket => basket.PurchaseProducts, x => x.MapFrom(dto => dto.PurchaseProducts));

            CreateMap<ShoppingCartDto, ShoppingCart>();

            CreateMap<ProductDto, Product>()
                .ForMember(product => product.Store, x => x.MapFrom(dto => new Store { Guid = dto.StoreGuid }));

            CreateMap<PurchaseProductDto, PurchaseProduct>()
                .ForMember(purchaseProduct => purchaseProduct.Product,
                    x => x.MapFrom(dto => new Product {Guid = dto.ProductGuid}))
                .ForMember(purchaseProduct => purchaseProduct.DiscountedPrice, x => x.MapFrom(dto => dto.Price));
            
            CreateMap<StorePurchaseDto, StorePurchase>()
                .ForMember(store => store.Buyer, x => x.MapFrom(dto => new User {Guid = dto.BuyerGuid}))
                .ForMember(store => store.DiscountedPrice, x => x.MapFrom(dto => dto.TotalPrice))
                .ForMember(store => store.Store, x => x.MapFrom(dto => new Store {Guid = dto.StoreGuid}));
            
            CreateMap<PurchaseDto, Purchase>()
                .ForMember(purchase => purchase.Buyer, x => x.MapFrom(dto => new User {Guid = dto.BuyerGuid}));
            
            CreateMap<StoreManagementDto, StoreManagement>()
                .ForMember(x => x.Permissions, x => x.Condition(xx => xx.Permissions != null));

            CreateMap<StoreOwnershipDto, StoreOwnership>();

            CreateMap<StoreManagementPermissionsDto, StoreManagementPermissions>();

            CreateMap<BaseEntityDto, BaseEntity>()
                .IncludeAllDerived()
                .ForMember(x => x.Guid, x => x.Condition(xx => xx.Guid != default));


            CreateMap<AdminUserDto, AdminUser>();
            CreateMap<NotificationDto, Notification>();
            CreateMap<StorePurchaseNotificationDto, StorePurchaseNotification>();

            CreateMap<PolicyDto, Policy>()
                .Include<AgeRestrictionPolicyDto, AgeRestrictionPolicy>()
                .Include<ProductAmountPolicyDto, Policy>()
                .Include<CategoryAmountPolicyDto, Policy>()
                .Include<CompositePolicyDto, CompositePolicy>()
                .Include<BinaryPolicyDto, BinaryPolicy>()
                .Include<TotalAmountPolicyDto, Policy>();

            CreateMap<ProductAmountPolicyDto, Policy>()
                .ConstructUsing((policyDto, _) =>
                    policyDto.Type switch
                    {
                        PolicyType.MaxProductAmount => new MaxProductAmountPolicy(new Product { Guid = policyDto.ProductGuid }, policyDto.Amount),
                        PolicyType.MinProductAmount => new MinProductAmountPolicy(new Product { Guid = policyDto.ProductGuid }, policyDto.Amount),
                        _ => throw new ArgumentOutOfRangeException()
                    });

            CreateMap<CategoryAmountPolicyDto, Policy>()
                .ConstructUsing((policyDto, _) =>
                    policyDto.Type switch
                    {
                        PolicyType.MaxCategoryAmount => new MaxCategoryAmountPolicy(policyDto.Category, policyDto.Amount),
                        PolicyType.MinCategoryAmount => new MinCategoryAmountPolicy(policyDto.Category, policyDto.Amount),
                        _ => throw new ArgumentOutOfRangeException()
                    });

            CreateMap<TotalAmountPolicyDto, Policy>()
                .ConstructUsing((policyDto, _) =>
                    policyDto.Type switch
                    {
                        PolicyType.MaxTotalAmount => new MaxTotalAmountPolicy(policyDto.Amount),
                        PolicyType.MinTotalAmount => new MinTotalAmountPolicy(policyDto.Amount),
                        _ => throw new ArgumentOutOfRangeException()
                    });

            CreateMap<AgeRestrictionPolicyDto, AgeRestrictionPolicy>()
                .ConstructUsing((policyDto, _) => new AgeRestrictionPolicy(new Product {Guid = policyDto.ProductGuid}, policyDto.MinAge));

            CreateMap<CompositePolicyDto, CompositePolicy>()
                .ConstructUsing((policyDto, context) => new CompositePolicy(context.Mapper.Map<PolicyOperator>(policyDto.Operator)));

            CreateMap<BinaryPolicyDto, BinaryPolicy>()
                .ConstructUsing((policyDto, context) => new BinaryPolicy(context.Mapper.Map<PolicyOperator>(policyDto.Operator)));

            CreateMap<OperatorType, PolicyOperator>()
                .ConstructUsing((type, _) => 
                    type switch
                    {
                        OperatorType.And => new AndPolicyOperator(),
                        OperatorType.Or => new OrPolicyOperator(),
                        OperatorType.Condition => new ConditionPolicyOperator(),
                        OperatorType.Xor => new XorPolicyOperator(),
                        _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
                    });

            CreateMap<DiscountDto, Discount>()
                .Include<ProductDiscountDto, ProductDiscount>()
                .Include<CategoryDiscountDto, CategoryDiscount>()
                .Include<BasketDiscountDto, BasketDiscount>()
                .Include<CompositeDiscountDto, CompositeDiscount>();

            CreateMap<CategoryDiscountDto, CategoryDiscount>();

            CreateMap<BasketDiscountDto, BasketDiscount>();
                

            CreateMap<ProductDiscountDto, ProductDiscount>()
                .ConstructUsing((discountDto, _) => new ProductDiscount(new Product { Guid = discountDto.ProductGuid }));

            CreateMap<CompositeDiscountDto, CompositeDiscount>()
                .ConstructUsing((discountDto, context) => new CompositeDiscount(context.Mapper.Map<DiscountOperator>(discountDto.Operator)));

            CreateMap<OperatorTypeDiscount, DiscountOperator>()
                .ConstructUsing((type, _) =>
                    type switch
                    {
                        OperatorTypeDiscount.Max => new MaxDiscountOperator(),
                        OperatorTypeDiscount.Sum => new SumDiscountOperator(),
                        _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
                    });
        }
    }
}
