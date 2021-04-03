using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
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
            CreateMap<PurchaseDto, Purchase>();
            CreateMap<ProductDto, Product>();
            CreateMap<PurchaseProductDto, PurchaseProduct>();
            CreateMap<UserDto, User>();
        }
    }
}
