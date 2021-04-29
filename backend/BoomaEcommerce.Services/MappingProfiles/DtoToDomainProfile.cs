using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using BoomaEcommerce.Core;
using BoomaEcommerce.Domain;
using BoomaEcommerce.Services.DTO;

namespace BoomaEcommerce.Services.MappingProfiles
{
    /// <summary>
    /// A class that holds all the mappings from DTO objects to Domain objects.
    /// </summary>
    public class DtoToDomainProfile : Profile
    {
        public DtoToDomainProfile()
        {
            CreateMap<UserDto, User>();

            CreateMap<StoreDto, Store>()
                .ForMember(store => store.StoreFounder, x => x.MapFrom(dto => new User {Guid = dto.FounderUserGuid}));

            CreateMap<List<PurchaseProductDto>, ConcurrentDictionary<Guid, PurchaseProduct>>()
                .ConstructUsing((x, y) => new ConcurrentDictionary<Guid, PurchaseProduct>(
                    x.Select(ppDto => y.Mapper.Map<PurchaseProduct>(ppDto))
                        .ToDictionary(pp => pp.Guid)));

            CreateMap<ShoppingBasketDto, ShoppingBasket>()
                .ForMember(basket => basket.Store, x => x.MapFrom(dto => new Store {Guid = dto.StoreGuid}))
                .ForMember(basket => basket.PurchaseProducts, x => x.MapFrom(dto => dto.PurchaseProducts));

            CreateMap<ShoppingCartDto, ShoppingCart>();

            CreateMap<ProductDto, Product>()
                .ForMember(product => product.Store, x => x.MapFrom(dto => new Store { Guid = dto.StoreGuid }));

            CreateMap<PurchaseProductDto, PurchaseProduct>()
                .ForMember(purchaseProduct => purchaseProduct.Product,
                    x => x.MapFrom(dto => new Product {Guid = dto.ProductGuid}));

            CreateMap<StorePurchaseDto, StorePurchase>()
                .ForMember(store => store.Buyer, x => x.MapFrom(dto => new User { Guid = dto.BuyerGuid }))
                .ForMember(store => store.Store, x => x.MapFrom(dto => new Store { Guid = dto.StoreGuid }));

            CreateMap<PurchaseDto, Purchase>()
                .ForMember(purchase => purchase.Buyer, x => x.MapFrom(dto => new User {Guid = dto.BuyerGuid}));

            CreateMap<StorePurchaseDto, StorePurchase>()
                .ForMember(storePurchase => storePurchase.Store, x => x.MapFrom(dto => new Store { Guid = dto.StoreGuid }))
                .ForMember(storePurchase => storePurchase.Buyer, x => x.MapFrom(dto => new User { Guid = dto.BuyerGuid }));

            CreateMap<StoreManagementDto, StoreManagement>()
                .ForMember(x => x.Permissions, x => x.Condition(xx => xx.Permissions != null));

            CreateMap<StoreOwnershipDto, StoreOwnership>();

            CreateMap<StoreManagementPermissionDto, StoreManagementPermission>();

            CreateMap<BaseEntityDto, BaseEntity>()
                .IncludeAllDerived()
                .ForMember(x => x.Guid, x => x.Condition(xx => xx.Guid != default));
        }
    }
}
