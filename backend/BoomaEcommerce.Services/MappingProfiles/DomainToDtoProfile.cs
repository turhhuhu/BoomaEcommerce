using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using BoomaEcommerce.Core;
using BoomaEcommerce.Domain;
using BoomaEcommerce.Domain.Policies;
using BoomaEcommerce.Domain.Policies.Operators;
using BoomaEcommerce.Domain.Policies.PolicyTypes;
using BoomaEcommerce.Services.DTO;
using BoomaEcommerce.Services.DTO.Policies;

namespace BoomaEcommerce.Services.MappingProfiles
{
    public class DomainToDtoProfile : Profile
    {
        /// <summary>
        /// A class that holds all the mappings from Domain objects to DTO objects.
        /// </summary>
        public DomainToDtoProfile()
        {
            CreateMap<Store, StoreDto>()
                .ForMember(dto => dto.FounderUserGuid, x => x.MapFrom(store => store.StoreFounder.Guid));

            CreateMap<User, UserDto>();

            CreateMap<ShoppingCart, ShoppingCartDto>()
                .ForMember(cartDto => cartDto.Baskets, x => x.MapFrom(shoppingCart => shoppingCart.StoreGuidToBaskets.Values));

            CreateMap<ShoppingBasket, ShoppingBasketDto>()
                  .ForMember(basketDto =>basketDto.PurchaseProducts , x => x.MapFrom(shoppingBasket => shoppingBasket.PurchaseProducts.Values))
                  .ForMember(basketDto => basketDto.StoreGuid, x => x.MapFrom(basket => basket.Store.Guid));

            CreateMap<StoreManagement, StoreManagementDto>();

            CreateMap<StoreOwnership, StoreOwnershipDto>();

            CreateMap<Product, ProductDto>()
                .ForMember(dto => dto.StoreGuid, x => x.MapFrom(product => product.Store.Guid));

            CreateMap<StorePurchase, StorePurchaseDto>()
                .ForMember(dto => dto.StoreGuid, x => x.MapFrom(storePurchase => storePurchase.Store.Guid))
                .ForMember(dto => dto.BuyerGuid, x => x.MapFrom(storePurchase => storePurchase.Buyer.Guid));

            CreateMap<PurchaseProduct, PurchaseProductDto>()
                .ForMember(dto => dto.ProductGuid, x => x.MapFrom(purchaseProduct => purchaseProduct.Product.Guid));
                
            CreateMap<Purchase, PurchaseDto>()
                .ForMember(dto => dto.BuyerGuid, x => x.MapFrom(purchase => purchase.Buyer.Guid));
            
            CreateMap<StoreManagementPermissions, StoreManagementPermissionsDto>();

            CreateMap<Notification, NotificationDto>()
                .Include<StorePurchaseNotification, StorePurchaseNotificationDto>()
                .Include<RoleDismissalNotification, RoleDismissalNotificationDto>();

            CreateMap<StorePurchaseNotification, StorePurchaseNotificationDto>()
                .ForMember(dto => dto.BuyerMetaData, x => x.MapFrom(n => n.Buyer))
                .ForMember(dto => dto.StoreMetaData, x => x.MapFrom(n => n.Store));

            CreateMap<RoleDismissalNotification, RoleDismissalNotificationDto>()
                .ForMember(dto => dto.DismissingUserMetaData, x => x.MapFrom(n => n.DismissingUser))
                .ForMember(dto => dto.StoreMetaData, x => x.MapFrom(n => n.Store));

            CreateMap<User, BasicUserInfoDto>();
            CreateMap<User, UserMetaData>();
            CreateMap<Store, StoreMetaData>();

            CreateMap<Policy, PolicyDto>()
                .Include<AgeRestrictionPolicy, AgeRestrictionPolicyDto>()
                .Include<MaxProductAmountPolicy, ProductAmountPolicyDto>()
                .Include<MinProductAmountPolicy, ProductAmountPolicyDto>()
                .Include<MaxCategoryAmountPolicy, CategoryAmountPolicyDto>()
                .Include<MinCategoryAmountPolicy, CategoryAmountPolicyDto>()
                .Include<CompositePolicy, CompositePolicyDto>();

            CreateMap<AgeRestrictionPolicy, AgeRestrictionPolicyDto>()
                .ForMember(policyDto => policyDto.ProductGuid, x => x.MapFrom(policy => policy.Product.Guid));

            CreateMap<MaxProductAmountPolicy, ProductAmountPolicyDto>()
                .ForMember(policyDto => policyDto.ProductGuid, x => x.MapFrom(policy => policy.Product.Guid))
                .ForMember(policyDto => policyDto.Amount, x => x.MapFrom(policy => policy.MaxAmount))
                .ForMember(policyDto => policyDto.Type, x => x.MapFrom(_ => PolicyType.MaxProductAmount));

            CreateMap<MinProductAmountPolicy, ProductAmountPolicyDto>()
                .ForMember(policyDto => policyDto.ProductGuid, x => x.MapFrom(policy => policy.Product.Guid))
                .ForMember(policyDto => policyDto.Amount, x => x.MapFrom(policy => policy.MinAmount))
                .ForMember(policyDto => policyDto.Type, x => x.MapFrom(_ => PolicyType.MinProductAmount));

            CreateMap<MaxCategoryAmountPolicy, CategoryAmountPolicyDto>()
                .ForMember(policyDto => policyDto.Amount, x => x.MapFrom(policy => policy.MaxAmount))
                .ForMember(policyDto => policyDto.Type, x => x.MapFrom(_ => PolicyType.MaxCategoryAmount));

            CreateMap<MinCategoryAmountPolicy, CategoryAmountPolicyDto>()
                .ForMember(policyDto => policyDto.Amount, x => x.MapFrom(policy => policy.MinAmount))
                .ForMember(policyDto => policyDto.Type, x => x.MapFrom(_ => PolicyType.MinCategoryAmount));

            CreateMap<CompositePolicy, CompositePolicyDto>()
                .ConstructUsing((policy, context) => new CompositePolicyDto
                {
                    Operator = context.Mapper.Map<OperatorType>(policy.Operator),
                    SubPolicies = context.Mapper.Map<IEnumerable<PolicyDto>>(policy.GetSubPolicies())
                });

            CreateMap<PolicyOperator, OperatorType>()
                .ConstructUsing((@operator, _) =>
                    @operator switch
                    {
                        AndPolicyOperator => OperatorType.And,
                        ConditionPolicyOperator => OperatorType.Condition,
                        OrPolicyOperator => OperatorType.Or,
                        XorPolicyOperator => OperatorType.Xor,
                        _ => throw new ArgumentOutOfRangeException(nameof(@operator))
                    });
        }
    }
}
