using System;
using System.Collections.Generic;
using AutoMapper;
using BoomaEcommerce.Services.Tests;
using BoomaEcommerce.Domain;
using BoomaEcommerce.Services.DTO;
using BoomaEcommerce.Services.MappingProfiles;
using BoomaEcommerce.Services.Users;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace BoomaEcommerce.Services.Tests
{
    public class UsersServiceTests
    {
        
        public MapperConfiguration config = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile(new DomainToDtoProfile());
            cfg.AddProfile(new DtoToDomainProfile());
        });
        
        

    }
}