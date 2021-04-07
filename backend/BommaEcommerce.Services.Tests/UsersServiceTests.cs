using System;
using System.Collections.Generic;
using AutoMapper;
using BommaEcommerce.Services.Tests;
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
        public Dictionary<Guid, StoreManagement> _EntitiesStoreManagements = new Dictionary<Guid, StoreManagement>();

        public Dictionary<Guid, StoreOwnership> _EntitiesStoreOwnerships = new Dictionary<Guid, StoreOwnership>();

        public Dictionary<Guid, StoreManagementPermission> _EntitiesStoreManagementPermissions =
            new Dictionary<Guid, StoreManagementPermission>();

        static readonly MapperConfiguration Config = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile(new DomainToDtoProfile());
            cfg.AddProfile(new DtoToDomainProfile());
        });

        IMapper Mapper = Config.CreateMapper();

        [Fact]
        public async void GetSellersInformationTest()
        {
            Mock<ILogger<UsersService>> loggerMock = new Mock<ILogger<UsersService>>();


            Store s1 = new() { StoreName = "Nike" };
            Store s2 = new() { StoreName = "Adidas" };
            Store s3 = new() { StoreName = "Fake Store" };

            User u1 = new() {UserName = "Benny"};
            User u2 = new() {UserName = "Matan"};
            User u3 = new() { UserName = "Omer" };
            User u4 = new() { UserName = "Arye" };

            StoreManagement sm1 = new() { Store = s1, User = u1};
            StoreManagement sm2 = new() { Store = s2, User = u2 };
            StoreOwnership so3 = new() { Store = s1, User = u3 };
            StoreOwnership so4 = new() { Store = s2, User = u4 };

            _EntitiesStoreManagements[sm1.Guid] = sm1;
            _EntitiesStoreManagements[sm2.Guid] = sm2;
            _EntitiesStoreOwnerships[so3.Guid] = so3;
            _EntitiesStoreOwnerships[so4.Guid] = so4;


            var repoOwnerships = DalMockFactory.MockRepository(_EntitiesStoreOwnerships);

            var repoManagements = DalMockFactory.MockRepository(_EntitiesStoreManagements);

            UsersService us = new (Mapper, loggerMock.Object, repoOwnerships.Object, repoManagements.Object);

            List<StoreManagementDto> lsm = new List<StoreManagementDto>();
            lsm.Add(Mapper.Map<StoreManagementDto>(sm1));

            List<StoreOwnershipDto> lso = new List<StoreOwnershipDto>();
            lso.Add(Mapper.Map<StoreOwnershipDto>(so3));

            var exp1 = new StoreSellersResponse
            {
                StoreManagers = lsm,
                StoreOwners = lso
            };

            var res1 = await us.GetAllSellersInformation(s1.Guid);


            List<StoreManagementDto> lsm2 = new List<StoreManagementDto>();
            lsm2.Add(Mapper.Map<StoreManagementDto>(sm2));

            List<StoreOwnershipDto> lso2 = new List<StoreOwnershipDto>();
            lso2.Add(Mapper.Map<StoreOwnershipDto>(so4));

            var exp2 = new StoreSellersResponse
            {
                StoreManagers = lsm2,
                StoreOwners = lso2

            };

            var res2 = await us.GetAllSellersInformation(s2.Guid);
            res2.Should().BeEquivalentTo(exp2);

            var res3 = await us.GetAllSellersInformation(s3.Guid);
            res3.StoreOwners.Should().BeEmpty();
            res3.StoreManagers.Should().BeEmpty();
        }

        [Fact]
        public async void GetPermissionsTest()
        {
            Mock<ILogger<UsersService>> loggerMock = new Mock<ILogger<UsersService>>();


            Store s1 = new() { StoreName = "Nike" };

            User u1 = new() { UserName = "Benny" };
            User u2 = new() { UserName = "Omer" };

            StoreManagement sm1 = new() { Store = s1, User = u1 };
            StoreManagement sm2 = new() { Store = s1, User = u2 };

            StoreManagementPermission smp1 = new() {CanDoSomething = true, StoreManagement = sm1};
            StoreManagementPermission smp2 = new() { CanDoSomething = false, StoreManagement = sm2 };

            _EntitiesStoreManagementPermissions[sm1.Guid] = smp1;
            _EntitiesStoreManagementPermissions[sm2.Guid] = smp2;


            var reopPermissions = DalMockFactory.MockRepository(_EntitiesStoreManagementPermissions);

            UsersService us = new(Mapper, loggerMock.Object, reopPermissions.Object);

            var res1 = await us.GetPermissions(smp1.Guid);
            var res2 = await us.GetPermissions(smp2.Guid);
            var res3 = await us.GetPermissions(new Guid());

            var r1 = Mapper.Map<StoreManagementPermission>(res1);

            var r2 = Mapper.Map<StoreManagementPermission>(res2);

            var r3 = Mapper.Map<StoreManagementPermission>(res3);

            r1.CanDoSomething.Should().BeTrue();

            r2.CanDoSomething.Should().BeFalse();

            r3.Should().BeNull();
        }

        [Fact]
        public async void UpdatePermissionsTest()
        {
            Mock<ILogger<UsersService>> loggerMock = new Mock<ILogger<UsersService>>();


            Store s1 = new() { StoreName = "Nike" };

            User u1 = new() { UserName = "Benny" };
            User u2 = new() { UserName = "Omer" };

            StoreManagement sm1 = new() { Store = s1, User = u1 };
            StoreManagement sm2 = new() { Store = s1, User = u2 };

            StoreManagementPermission smp1 = new() { CanDoSomething = true, StoreManagement = sm1 };
            StoreManagementPermission smp2 = new() { CanDoSomething = false, StoreManagement = sm2 };

            _EntitiesStoreManagementPermissions[smp1.Guid] = smp1;
            _EntitiesStoreManagementPermissions[smp2.Guid] = smp2;


            var repoPermissions = DalMockFactory.MockRepository(_EntitiesStoreManagementPermissions);

            UsersService us = new(Mapper, loggerMock.Object, repoPermissions.Object);

            var replace1 = await us.GetPermissions(smp1.Guid);
            replace1.CanDoSomething = false;

            var replace2 = await us.GetPermissions(smp2.Guid);
            replace2.CanDoSomething = true;

            await us.UpdatePermission(replace1);
            await us.UpdatePermission(replace2);

            var res1 = await us.GetPermissions(smp1.Guid);
            var res2 = await us.GetPermissions(smp2.Guid);
            var res3 = await us.GetPermissions(new Guid());

            var r1 = Mapper.Map<StoreManagementPermission>(res1);

            var r2 = Mapper.Map<StoreManagementPermission>(res2);

            var r3 = Mapper.Map<StoreManagementPermission>(res3);

            r1.CanDoSomething.Should().BeFalse();

            r2.CanDoSomething.Should().BeTrue();

            r3.Should().BeNull();
        }
    }
}