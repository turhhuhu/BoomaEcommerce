using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using BoomaEcommerce.Api.Responses;
using BoomaEcommerce.Services.DTO;

namespace BoomaEcommerce.Api
{
    public class DtoToResponseMappingProfile : Profile
    {
        public DtoToResponseMappingProfile()
        {
            CreateMap<StoreOwnershipDto, OwnerShipRoleResponse>()
                .ForMember(response => response.StoreMetaData, x => x.MapFrom(dto => dto.Store))
                .ForMember(response => response.UserMetaData, x => x.MapFrom(dto => dto.User));

            CreateMap<StoreManagementDto, ManagementRoleResponse>()
                .ForMember(response => response.StoreMetaData, x => x.MapFrom(dto => dto.Store))
                .ForMember(response => response.UserMetaData, x => x.MapFrom(dto => dto.User));

            CreateMap<StoreSellersDto, StoreSellersResponse>()
                .ForMember(sellers => sellers.StoreManagers, x => x.MapFrom(dto => dto.StoreManagers))
                .ForMember(sellers => sellers.StoreOwners, x => x.MapFrom(dto => dto.StoreOwners));

            CreateMap<StoreDto, StoreMetaData>()
                .ForMember(metaData => metaData.StoreGuid, x => x.MapFrom(dto => dto.Guid));

            CreateMap<UserDto, UserMetaData>()
                .ForMember(metaData => metaData.UserGuid, x => x.MapFrom(dto => dto.Guid));
        }
    }
}
