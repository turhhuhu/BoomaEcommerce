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
        public Mock<ILogger<UsersService>> loggerMock = new Mock<ILogger<UsersService>>();
        [Fact]
        public async void NominateNewStoreOwnerTest_shouldReturnTrue_ifTheConditionsAreMet()
        {
            var mapper = config.CreateMapper();
            
            Dictionary<Guid, StoreOwnership> EntitiesOwnerships = new Dictionary<Guid, StoreOwnership>();
            Dictionary<Guid, StoreManagement> EntitiesManagements = new Dictionary<Guid, StoreManagement>();

            Store store1 = new (){StoreName = "nike"};
            Store store2 = new (){StoreName = "adidas"};
            
            User u1 = new() {Name = "Matan"};
            User u2 = new() {Name = "Benny"};
            User u3 = new() {Name = "Omer"};
            User u4 = new() {Name = "ori"}; 
            User u5 = new() {Name = "arik"}; 
            
            StoreOwnership s11 = new StoreOwnership(){Store = store1,User = u1};
            StoreOwnership s12 = new StoreOwnership(){Store = store2,User = u1};

            StoreOwnership s2 = new StoreOwnership(){Store = store2,User = u2};
            StoreManagement s3 = new StoreManagement(){Store = store2,User = u3};

            EntitiesOwnerships[s11.Guid]=s11;
            EntitiesOwnerships[s12.Guid]=s12;
            EntitiesOwnerships[s2.Guid]=s2;
            EntitiesManagements[s3.Guid] = s3;
                
            var repoOwnerships = DalMockFactory.MockRepository(EntitiesOwnerships);
            var repoManagements = DalMockFactory.MockRepository(EntitiesManagements);
            var us = new UsersService(mapper,loggerMock.Object,repoOwnerships.Object, repoManagements.Object );

            
            //FAIL:u2 is an owner of another store
            StoreOwnership s4 = new StoreOwnership(){Store = store1,User = u4};
            var newOwner = mapper.Map<StoreOwnershipDto>(s4);
            var result2 = await us.NominateNewStoreOwner(u2.Guid,newOwner);
            result2.Should().BeFalse();
            
            //SUCCESS
            StoreOwnership s22 = new StoreOwnership(){Store = store1,User = u2};
            newOwner = mapper.Map<StoreOwnershipDto>(s22);
            var result1 = await us.NominateNewStoreOwner(u1.Guid,newOwner);
            result1.Should().BeTrue();
            
            
            newOwner = mapper.Map<StoreOwnershipDto>(s4);
            var result3 = await us.NominateNewStoreOwner(u5.Guid,newOwner);//FAIL:u5 is not an owner wanted store
            result3.Should().BeFalse();
            
            
            newOwner = mapper.Map<StoreOwnershipDto>(s2);
            var result4 = await us.NominateNewStoreOwner(u1.Guid,newOwner);//Fail : both are owners
            result4.Should().BeFalse();
            
            StoreOwnership s32 = new StoreOwnership(){Store = store2,User = u3};
            newOwner = mapper.Map<StoreOwnershipDto>(s32);
            var result5 = await us.NominateNewStoreOwner(u2.Guid,newOwner);//Fail : already a manager
            result5.Should().BeFalse();

        }
        
        
        [Fact]
        public async void NominateNewStoreManagerTest_shouldReturnTrue_ifTheConditionsAreMet()
        {
            
            var mapper = config.CreateMapper();
            
            Dictionary<Guid, StoreOwnership> EntitiesOwnerships = new Dictionary<Guid, StoreOwnership>();
            Dictionary<Guid, StoreManagement> EntitiesManagements = new Dictionary<Guid, StoreManagement>();

            Store store1 = new (){StoreName = "nike"};
            Store store2 = new (){StoreName = "adidas"};
            
            User u1 = new() {Name = "Matan"};
            User u2 = new() {Name = "Benny"};
            User u3 = new() {Name = "Omer"};
            User u4 = new() {Name = "ori"}; 
            User u5 = new() {Name = "arik"}; 
            
            StoreOwnership s11 = new StoreOwnership(){Store = store1,User = u1};
            StoreOwnership s12 = new StoreOwnership(){Store = store2,User = u1};

            StoreOwnership s2 = new StoreOwnership(){Store = store2,User = u2};
            StoreManagement s3 = new StoreManagement(){Store = store2,User = u3};

            EntitiesOwnerships[s11.Guid]=s11;
            EntitiesOwnerships[s12.Guid]=s12;
            EntitiesOwnerships[s2.Guid]=s2;
            EntitiesManagements[s3.Guid] = s3;
                
            var repoOwnerships = DalMockFactory.MockRepository(EntitiesOwnerships);
            var repoManagements = DalMockFactory.MockRepository(EntitiesManagements);
            var us = new UsersService(mapper,loggerMock.Object,repoOwnerships.Object, repoManagements.Object );

            
            //FAIL:u2 is an owner of another store
            StoreManagement s4 = new StoreManagement(){Store = store1,User = u4};
            var newManager = mapper.Map<StoreManagementDto>(s4);
            var result2 = await us.NominateNewStoreManager(u2.Guid,newManager);
            result2.Should().BeFalse();
            
            //SUCCESS
            StoreManagement s22 = new StoreManagement(){Store = store1,User = u2};
            newManager = mapper.Map<StoreManagementDto>(s22);
            var result1 = await us.NominateNewStoreManager(u1.Guid,newManager);
            result1.Should().BeTrue();
            
            StoreManagement s41 = new StoreManagement(){Store = store1,User = u4};
            newManager = mapper.Map<StoreManagementDto>(s41);
            var result3 = await us.NominateNewStoreManager(u5.Guid,newManager);//FAIL:u5 is not an owner wanted store
            result3.Should().BeFalse();
            
            StoreManagement m22 = new StoreManagement(){Store = store1,User = u2};
            newManager = mapper.Map<StoreManagementDto>(m22);
            var result4 = await us.NominateNewStoreManager(u1.Guid,newManager);//Fail : both are owners
            result4.Should().BeFalse();
            
            StoreManagement s32 = new StoreManagement(){Store = store2,User = u3};
            newManager = mapper.Map<StoreManagementDto>(s32);
            var result5 = await us.NominateNewStoreManager(u2.Guid,newManager);//Fail : already a manager
            result5.Should().BeFalse();
        }


        

    }
}