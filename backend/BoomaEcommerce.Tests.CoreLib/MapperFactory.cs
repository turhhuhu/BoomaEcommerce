using AutoMapper;
using BoomaEcommerce.Services.MappingProfiles;
using BoomaEcommerce.Api;

namespace BoomaEcommerce.Tests.CoreLib
{
    public static class MapperFactory
    {
        public static IMapper GetMapper()
        {
            var mapperConfig = new MapperConfiguration(x =>
            {
                x.AddProfile(new DtoToDomainProfile());
                x.AddProfile(new DomainToDtoProfile());
                x.AddProfile(new DtoToResponseMappingProfile());
                x.AddProfile(new RequestToDtoMappingProfile());

            });
            var mapper = mapperConfig.CreateMapper();
            return mapper;
        }

    }
}