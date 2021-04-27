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

            CreateMap<ShoppingBasket, ShoppingBasketDto>();

            CreateMap<ShoppingCart, ShoppingCartDto>();

            CreateMap<StoreManagement, StoreManagementDto>();

            CreateMap<StoreOwnership, StoreOwnershipDto>();

            CreateMap<Product, ProductDto>()
                .ForMember(dto => dto.StoreGuid, x => x.MapFrom(product => product.Store.Guid));

            CreateMap<StorePurchase, StorePurchaseDto>()
                .ForMember(dto => dto.StoreGuid, x => x.MapFrom(storePurchase => storePurchase.Store.Guid));

            CreateMap<PurchaseProduct, PurchaseProductDto>();
                
            CreateMap<Purchase, PurchaseDto>()
                .ForMember(dto => dto.BuyerGuid, x => x.MapFrom(purchase => purchase.Buyer.Guid));

            CreateMap<StoreManagementPermission, StoreManagementPermissionDto>();
        }
    }
}
