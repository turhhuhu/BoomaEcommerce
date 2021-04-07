using System;
using System.Collections.Generic;
using System.Threading;
using AutoFixture;
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
        
        static readonly MapperConfiguration Config = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile(new DomainToDtoProfile());
            cfg.AddProfile(new DtoToDomainProfile());
        });

        IMapper Mapper = Config.CreateMapper();

        [Fact]
        public async void GetSellersInformationTest_ShouldReturnStoreSellers_WhenStoreGuidIsValid()
        {
            Mock<ILogger<UsersService>> loggerMock = new Mock<ILogger<UsersService>>();
            Dictionary<Guid, StoreManagement> _EntitiesStoreManagements = new Dictionary<Guid, StoreManagement>();
            Dictionary<Guid, StoreOwnership> _EntitiesStoreOwnerships = new Dictionary<Guid, StoreOwnership>();

            Store s1 = GetStoreData("Nike");

            User u1 = GetUserData("Benny", "Skidanov", "BennySkidanov");
            User u2 = GetUserData("Omer", "Kempner", "OmerKempner");
            User u3 = GetUserData("Matan", "Hazan", "MatanHazan");
            User u4 = GetUserData("Arye", "Shapiro", "BennySkidanov");

            StoreManagement sm1 = GetStoreManagementData(u1, s1);
            StoreManagement sm2 = GetStoreManagementData(u2, s1);
            StoreOwnership so3 = GetStoreOwnershipData(u3, s1);
            StoreOwnership so4 = GetStoreOwnershipData(u4, s1);

            _EntitiesStoreManagements[sm1.Guid] = sm1;
            _EntitiesStoreManagements[sm2.Guid] = sm2;
            _EntitiesStoreOwnerships[so3.Guid] = so3;
            _EntitiesStoreOwnerships[so4.Guid] = so4;


            var repoOwnerships = DalMockFactory.MockRepository(_EntitiesStoreOwnerships);

            var repoManagements = DalMockFactory.MockRepository(_EntitiesStoreManagements);

            UsersService us = new (Mapper, loggerMock.Object, repoOwnerships.Object, repoManagements.Object);

            List<StoreManagementDto> lsm = new List<StoreManagementDto>
            {
                Mapper.Map<StoreManagementDto>(sm1),
                Mapper.Map<StoreManagementDto>(sm2)
            };

            List<StoreOwnershipDto> lso = new List<StoreOwnershipDto>
            {
                Mapper.Map<StoreOwnershipDto>(so3),
                Mapper.Map<StoreOwnershipDto>(so4)
            };

            var expectedResponse = new StoreSellersResponse
            {
                StoreManagers = lsm,
                StoreOwners = lso
            };

            var response = await us.GetAllSellersInformation(s1.Guid);

            response.Should().BeEquivalentTo(expectedResponse);


            List<StoreManagementDto> lsm2 = new List<StoreManagementDto>();
            lsm2.Add(Mapper.Map<StoreManagementDto>(sm2));

            List<StoreOwnershipDto> lso2 = new List<StoreOwnershipDto>();
            lso2.Add(Mapper.Map<StoreOwnershipDto>(so4));
        }

        [Fact]
        public async void GetSellersInformationTest_ShouldReturnEmptyObject_WhenStoreGuidIsInvalid()
        {
            Mock<ILogger<UsersService>> loggerMock = new Mock<ILogger<UsersService>>();
            Dictionary<Guid, StoreManagement> _EntitiesStoreManagements = new Dictionary<Guid, StoreManagement>();
            Dictionary<Guid, StoreOwnership> _EntitiesStoreOwnerships = new Dictionary<Guid, StoreOwnership>();

            var repoOwnerships = DalMockFactory.MockRepository(_EntitiesStoreOwnerships);

            var repoManagements = DalMockFactory.MockRepository(_EntitiesStoreManagements);

            UsersService us = new(Mapper, loggerMock.Object, repoOwnerships.Object, repoManagements.Object);

            var response = await us.GetAllSellersInformation(new Guid());

            response.StoreOwners.Should().BeEmpty();
            response.StoreManagers.Should().BeEmpty();
        }

        [Fact]
        public async void GetPermissionsTest_ShouldReturnCorrectPermissions_WhenSMGuidIsValid()
        { 
            Dictionary<Guid, StoreManagementPermission> _EntitiesStoreManagementPermissions =
            new Dictionary<Guid, StoreManagementPermission>();

            Mock<ILogger<UsersService>> loggerMock = new Mock<ILogger<UsersService>>();


            Store s1 = GetStoreData("Adidas");

            User u1 = GetUserData("Benny", "Skidanov", "BennySkidanov");
            User u2 = GetUserData("Omer", "Kempner", "OmerKempner");

            StoreManagement sm1 = GetStoreManagementData(u1, s1);
            StoreManagement sm2 = GetStoreManagementData(u1, s1);

            StoreManagementPermission smp1 = GetStoreManagementPermissionData(true, sm1);
            StoreManagementPermission smp2 = GetStoreManagementPermissionData(false, sm2);

            _EntitiesStoreManagementPermissions[sm1.Guid] = smp1;
            _EntitiesStoreManagementPermissions[sm2.Guid] = smp2;


            var repoPermissions = DalMockFactory.MockRepository(_EntitiesStoreManagementPermissions);

            UsersService us = new(Mapper, loggerMock.Object, repoPermissions.Object);

            var res1 = await us.GetPermissions(smp1.Guid);
            var res2 = await us.GetPermissions(smp2.Guid);

            var r1 = Mapper.Map<StoreManagementPermission>(res1);
            var r2 = Mapper.Map<StoreManagementPermission>(res2);
            r1.CanDoSomething.Should().BeTrue();
            r2.CanDoSomething.Should().BeFalse();
        }

        [Fact]
        public async void GetPermissionsTest_ShouldReturnNull_WhenSMGuidIsInvalid()
        {
            Dictionary<Guid, StoreManagementPermission> _EntitiesStoreManagementPermissions =
                new Dictionary<Guid, StoreManagementPermission>();

            Mock<ILogger<UsersService>> loggerMock = new Mock<ILogger<UsersService>>();

            var repoPermissions = DalMockFactory.MockRepository(_EntitiesStoreManagementPermissions);

            UsersService us = new(Mapper, loggerMock.Object, repoPermissions.Object);

            var res1 = await us.GetPermissions(new Guid());

            var r1 = Mapper.Map<StoreManagementPermission>(res1);

            r1.Should().BeNull();
        }

        [Fact]
        public async void UpdatePermissionsTest_UpdatePermissionsCorrectly_WhenDtoIsValid()
        {
            Dictionary<Guid, StoreManagementPermission> _EntitiesStoreManagementPermissions =
                new Dictionary<Guid, StoreManagementPermission>();

            Mock<ILogger<UsersService>> loggerMock = new Mock<ILogger<UsersService>>();

            Store s1 = GetStoreData("MaccabiTelAvivFanStore");

            User u1 = GetUserData("Benny", "Skidanov", "BennySkidanov");

            StoreManagement sm1 = GetStoreManagementData(u1, s1);

            StoreManagementPermission smp1 = GetStoreManagementPermissionData(true, sm1);

            _EntitiesStoreManagementPermissions[smp1.Guid] = smp1;

            var repoPermissions = DalMockFactory.MockRepository(_EntitiesStoreManagementPermissions);

            UsersService us = new(Mapper, loggerMock.Object, repoPermissions.Object);

            var replace1 = await us.GetPermissions(smp1.Guid);
            replace1.CanDoSomething = false;

            await us.UpdatePermission(replace1);

            var res1 = await us.GetPermissions(smp1.Guid);

            var r1 = Mapper.Map<StoreManagementPermission>(res1);

            r1.CanDoSomething.Should().BeFalse();
        }

        [Fact]
        public async void UpdatePermissionsTest_UpdatePermissionNotUpdated_WhenSMDtoIsInvalid()
        {
            Dictionary<Guid, StoreManagementPermission> _EntitiesStoreManagementPermissions =
                new Dictionary<Guid, StoreManagementPermission>();

            Mock<ILogger<UsersService>> loggerMock = new Mock<ILogger<UsersService>>();

            Store s1 = GetStoreData("MaccabiTelAvivFanStore");

            User u1 = GetUserData("Benny", "Skidanov", "BennySkidanov");

            StoreManagement sm1 = GetStoreManagementData(u1, s1);

            StoreManagementPermission smp1 = GetStoreManagementPermissionData(true, sm1);

            _EntitiesStoreManagementPermissions[smp1.Guid] = smp1;

            var repoPermissions = DalMockFactory.MockRepository(_EntitiesStoreManagementPermissions);

            UsersService us = new(Mapper, loggerMock.Object, repoPermissions.Object);

            var replace1 = await us.GetPermissions(smp1.Guid);
            replace1.CanDoSomething = false;

            await us.UpdatePermission(new StoreManagementPermissionDto());

            var res1 = await us.GetPermissions(smp1.Guid);

            var r1 = Mapper.Map<StoreManagementPermission>(res1);

            r1.CanDoSomething.Should().BeTrue();
        }

        private static User GetUserData(string fName, string lName, string uname)
        {
            return new User() {Name = fName, LastName = lName, UserName = uname};
        }

        private static Store GetStoreData(string name)
        {
            return new Store() {StoreName = name};
        }

        private static StoreManagement GetStoreManagementData(User u, Store s)
        {
            return new StoreManagement() {User = u, Store = s};
        }

        private static StoreOwnership GetStoreOwnershipData(User u, Store s)
        {
            return new StoreOwnership() { User = u, Store = s };
        }

        private static StoreManagementPermission GetStoreManagementPermissionData(bool flag, StoreManagement sm)
        {
            return new StoreManagementPermission() {CanDoSomething = flag, StoreManagement = sm};
        }
    }

}