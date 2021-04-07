using AutoMapper;
using BoomaEcommerce.Services.MappingProfiles;

namespace BoomaEcommerce.Services.Tests
{
    public static class MapperFactory
    {
        public static IMapper GetMapper()
        {
            var mapperConfig = new MapperConfiguration(x =>
            {
                x.AddProfile(new DtoToDomainProfile());
                x.AddProfile(new DomainToDtoProfile());
            });
            var mapper = mapperConfig.CreateMapper();
            return mapper;
        }

    }
}