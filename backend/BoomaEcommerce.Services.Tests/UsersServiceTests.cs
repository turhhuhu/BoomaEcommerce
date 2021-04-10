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
        public async void GetSellersInformation_ShouldReturnStoreSellers_WhenStoreExists()
        {
            // Arrange 
            Mock<ILogger<UsersService>> loggerMock = new Mock<ILogger<UsersService>>();
            Dictionary<Guid, StoreManagement> entitiesStoreManagements = new Dictionary<Guid, StoreManagement>();
            Dictionary<Guid, StoreOwnership> entitiesStoreOwnerships = new Dictionary<Guid, StoreOwnership>();

            Store storeNike = GetStoreData("Nike");

            User uBenny = GetUserData("Benny", "Skidanov", "BennySkidanov");
            User uOmer = GetUserData("Omer", "Kempner", "OmerKempner");
            User uMatan = GetUserData("Matan", "Hazan", "MatanHazan");
            User uArye = GetUserData("Arye", "Shapiro", "BennySkidanov");

            StoreManagement smBenny = GetStoreManagementData(uBenny, storeNike);
            StoreManagement smOmer = GetStoreManagementData(uOmer, storeNike);
            StoreOwnership soMatan = GetStoreOwnershipData(uMatan, storeNike);
            StoreOwnership soArye = GetStoreOwnershipData(uArye, storeNike);

            entitiesStoreManagements[smBenny.Guid] = smBenny;
            entitiesStoreManagements[smOmer.Guid] = smOmer;
            entitiesStoreOwnerships[soMatan.Guid] = soMatan;
            entitiesStoreOwnerships[soArye.Guid] = soArye;


            var repoOwnerships = DalMockFactory.MockRepository(entitiesStoreOwnerships);

            var repoManagements = DalMockFactory.MockRepository(entitiesStoreManagements);

            UsersService us = new (Mapper, loggerMock.Object, repoOwnerships.Object, repoManagements.Object);

            List<StoreManagementDto> lsm = new List<StoreManagementDto>
            {
                Mapper.Map<StoreManagementDto>(smBenny),
                Mapper.Map<StoreManagementDto>(smOmer)
            };

            List<StoreOwnershipDto> lso = new List<StoreOwnershipDto>
            {
                Mapper.Map<StoreOwnershipDto>(soMatan),
                Mapper.Map<StoreOwnershipDto>(soArye)
            };

            var expectedResponse = new StoreSellersResponse
            {
                StoreManagers = lsm,
                StoreOwners = lso
            };

            // Act 
            var response = await us.GetAllSellersInformation(storeNike.Guid);

            // Assert
            response.Should().BeEquivalentTo(expectedResponse);
        }

        [Fact]
        public async void GetSellersInformation_ShouldReturnEmptyObject_WhenStoreDoesNotExist()
        {
            // Arrange 
            Mock<ILogger<UsersService>> loggerMock = new Mock<ILogger<UsersService>>();
            Dictionary<Guid, StoreManagement> entitiesStoreManagements = new Dictionary<Guid, StoreManagement>();
            Dictionary<Guid, StoreOwnership> entitiesStoreOwnerships = new Dictionary<Guid, StoreOwnership>();

            var repoOwnerships = DalMockFactory.MockRepository(entitiesStoreOwnerships);

            var repoManagements = DalMockFactory.MockRepository(entitiesStoreManagements);

            UsersService us = new(Mapper, loggerMock.Object, repoOwnerships.Object, repoManagements.Object);

            // Act 
            var response = await us.GetAllSellersInformation(new Guid()); // Store Guid does not exist !!

            // Assert 
            response.StoreOwners.Should().BeEmpty();
            response.StoreManagers.Should().BeEmpty();
        }

        [Fact]
        public async void GetPermissions_ShouldReturnCorrectPermissions_WhenSMExists()
        { 
            // Arrange 
            Dictionary<Guid, StoreManagementPermission> entitiesStoreManagementPermissions =
            new Dictionary<Guid, StoreManagementPermission>();

            Mock<ILogger<UsersService>> loggerMock = new Mock<ILogger<UsersService>>();


            Store s1 = GetStoreData("Adidas");

            User u1 = GetUserData("Benny", "Skidanov", "BennySkidanov");
            User u2 = GetUserData("Omer", "Kempner", "OmerKempner");

            StoreManagement sm1 = GetStoreManagementData(u1, s1);
            StoreManagement sm2 = GetStoreManagementData(u1, s1);

            StoreManagementPermission smp1 = GetStoreManagementPermissionData(true, sm1);
            StoreManagementPermission smp2 = GetStoreManagementPermissionData(false, sm2);

            entitiesStoreManagementPermissions[sm1.Guid] = smp1;
            entitiesStoreManagementPermissions[sm2.Guid] = smp2;


            var repoPermissions = DalMockFactory.MockRepository(entitiesStoreManagementPermissions);

            UsersService us = new(Mapper, loggerMock.Object, repoPermissions.Object);

            // Act 
            var res1 = await us.GetPermissions(smp1.Guid);
            var res2 = await us.GetPermissions(smp2.Guid);

            var r1 = Mapper.Map<StoreManagementPermission>(res1);
            var r2 = Mapper.Map<StoreManagementPermission>(res2);
            
            // Assert
            r1.CanDoSomething.Should().BeTrue();
            r2.CanDoSomething.Should().BeFalse();
        }

        [Fact]
        public async void GetPermissions_ShouldReturnNull_WhenSMDoesNotExist()
        {
            // Arrange 
            Dictionary<Guid, StoreManagementPermission> entitiesStoreManagementPermissions =
                new Dictionary<Guid, StoreManagementPermission>();

            Mock<ILogger<UsersService>> loggerMock = new Mock<ILogger<UsersService>>();

            var repoPermissions = DalMockFactory.MockRepository(entitiesStoreManagementPermissions);

            UsersService us = new(Mapper, loggerMock.Object, repoPermissions.Object);

            // Act 
            var res1 = await us.GetPermissions(new Guid());

            var r1 = Mapper.Map<StoreManagementPermission>(res1);

            // Assert
            r1.Should().BeNull();
        }

        [Fact]
        public async void UpdatePermissions_UpdatePermissionsCorrectly_WhenStoreManagerDtoExist()
        {
            // Arrange
            Dictionary<Guid, StoreManagementPermission> entitiesStoreManagementPermissions =
                new Dictionary<Guid, StoreManagementPermission>();

            Mock<ILogger<UsersService>> loggerMock = new Mock<ILogger<UsersService>>();

            Store s1 = GetStoreData("MaccabiTelAvivFanStore");

            User u1 = GetUserData("Benny", "Skidanov", "BennySkidanov");

            StoreManagement sm1 = GetStoreManagementData(u1, s1);

            StoreManagementPermission smp1 = GetStoreManagementPermissionData(true, sm1);

            entitiesStoreManagementPermissions[smp1.Guid] = smp1;

            var repoPermissions = DalMockFactory.MockRepository(entitiesStoreManagementPermissions);

            UsersService us = new(Mapper, loggerMock.Object, repoPermissions.Object);

            // Act 
            var replace1 = await us.GetPermissions(smp1.Guid);
            replace1.CanDoSomething = false;

            await us.UpdatePermission(replace1);

            var res1 = await us.GetPermissions(smp1.Guid);

            var r1 = Mapper.Map<StoreManagementPermission>(res1);

            // Assert
            r1.CanDoSomething.Should().BeFalse();
        }

        [Fact]
        public async void UpdatePermissions_UpdatePermissionNotUpdated_WhenSMDoesNotExist()
        {
            // Arrange
            Dictionary<Guid, StoreManagementPermission> entitiesStoreManagementPermissions =
                new Dictionary<Guid, StoreManagementPermission>();

            Mock<ILogger<UsersService>> loggerMock = new Mock<ILogger<UsersService>>();

            Store s1 = GetStoreData("MaccabiTelAvivFanStore");

            User u1 = GetUserData("Benny", "Skidanov", "BennySkidanov");

            StoreManagement sm1 = GetStoreManagementData(u1, s1);

            StoreManagementPermission smp1 = GetStoreManagementPermissionData(true, sm1);

            entitiesStoreManagementPermissions[smp1.Guid] = smp1;

            var repoPermissions = DalMockFactory.MockRepository(entitiesStoreManagementPermissions);

            UsersService us = new(Mapper, loggerMock.Object, repoPermissions.Object);

            // Act 
            var replace1 = await us.GetPermissions(smp1.Guid);
            replace1.CanDoSomething = false;

            await us.UpdatePermission(new StoreManagementPermissionDto());

            var res1 = await us.GetPermissions(smp1.Guid);

            var r1 = Mapper.Map<StoreManagementPermission>(res1);

            // Assert 
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