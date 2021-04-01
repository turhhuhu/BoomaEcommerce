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
    public class DomainToDtoProfile : Profile
    {
        /// <summary>
        /// A class that holds all the mappings from Domain objects to DTO objects.
        /// </summary>
        public DomainToDtoProfile()
        {
            CreateMap<Store, StoreDto>();
        }
    }
}
