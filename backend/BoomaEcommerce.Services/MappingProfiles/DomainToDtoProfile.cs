using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using BoomaEcommerce.Core;
using BoomaEcommerce.Domain;
using BoomaEcommerce.Services.DTO;

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

            CreateMap<StoreManagementPermission, StoreManagementPermissionDto>();

            CreateMap<StorePurchaseNotification, StorePurchaseNotificationDto>();
            CreateMap<Notification, NotificationDto>();
        }
    }
}
