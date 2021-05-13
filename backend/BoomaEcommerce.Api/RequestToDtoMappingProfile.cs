using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using BoomaEcommerce.Api.Requests;
using BoomaEcommerce.Services.DTO;

namespace BoomaEcommerce.Api
{
    public class RequestToDtoMappingProfile : Profile
    {
        public RequestToDtoMappingProfile()
        {
            CreateMap<CreateOwnershipRequest, StoreOwnershipDto>()
                .ForMember(dto => dto.Store, x => x.MapFrom(req => new StoreDto()))
                .ForMember(dto => dto.User, x => x.MapFrom(req => new UserDto {Guid = req.NominatedUserGuid}));

            CreateMap<CreateManagementRequest, StoreManagementDto>()
                .ForMember(dto => dto.Store, x => x.MapFrom(req => new StoreDto()))
                .ForMember(dto => dto.User, x => x.MapFrom(req => new UserDto { Guid = req.NominatedUserGuid }));
        }
    }
}
