﻿using AutoMapper;
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
            CreateMap<StoreDto, Store>();
            CreateMap<ShoppingBasketDto, ShoppingBasket>();
            CreateMap<ShoppingCartDto, ShoppingCart>();
            CreateMap<ProductDto, Product>();
            CreateMap<StoreDto,Store>();
            CreateMap<PurchaseProductDto, PurchaseProduct>().ForMember(dest => dest.Product,
                opt =>
                    opt.MapFrom(x => new Product{Guid = x.ProductDto.Guid}));
            CreateMap<StorePurchaseDto, StorePurchase>();
            CreateMap<PurchaseDto, Purchase>();
            CreateMap<StorePurchaseDto, StorePurchase>();   
            CreateMap<StoreManagementDto, StoreManagement>();
            CreateMap<StoreOwnershipDto, StoreOwnership>();
            CreateMap<StoreManagementPermissionDto, StoreManagementPermission>();

        }
    }
}
