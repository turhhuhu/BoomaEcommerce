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
        public async void NominateNewStoreOwnerTest_ReturnTrue_WhenAnOwnerNominatesNewOwnerThatDoesntHaveOtherResponsibility()
        {
            var mapper = config.CreateMapper();
            
            Dictionary<Guid, StoreOwnership> EntitiesOwnerships = new Dictionary<Guid, StoreOwnership>();
            Dictionary<Guid, StoreManagement> EntitiesManagements = new Dictionary<Guid, StoreManagement>();

            Store store1 = createStoreObject("nike");
            Store store2 = createStoreObject("adidas");

            User u1 = createUserObject("Matan");
            User u2 = createUserObject("Benny");
            User u3 = createUserObject("Omer");
            User u4 = createUserObject("Ori");
            User u5 = createUserObject("Arik");
            
            StoreOwnership s11 = createStoreOwnershipObject(store1,u1);
            StoreOwnership s12 = createStoreOwnershipObject(store2,u1);

            StoreOwnership s2 = createStoreOwnershipObject(store2,u2);
            StoreManagement s3 = createStoreManagementObject(store2,u3);

            EntitiesOwnerships[s11.Guid]=s11;
            EntitiesOwnerships[s12.Guid]=s12;
            EntitiesOwnerships[s2.Guid]=s2;
            EntitiesManagements[s3.Guid] = s3;
                
            var repoOwnerships = DalMockFactory.MockRepository(EntitiesOwnerships);
            var repoManagements = DalMockFactory.MockRepository(EntitiesManagements);
            var us = new UsersService(mapper,loggerMock.Object,repoOwnerships.Object, repoManagements.Object );

            
            //SUCCESS
            StoreOwnership s22 = createStoreOwnershipObject(store1,u2);
            var newOwner = mapper.Map<StoreOwnershipDto>(s22);
            var result1 = await us.NominateNewStoreOwner(u1.Guid,newOwner);
            result1.Should().BeTrue();
            

        }
        [Fact]
        public async void NominateNewStoreOwnerTest_ReturnFalse_ifOwnerOfAStoreTriesToNominateNewOwnerToAnotherStore()
        {
            var mapper = config.CreateMapper();
            
            Dictionary<Guid, StoreOwnership> EntitiesOwnerships = new Dictionary<Guid, StoreOwnership>();
            Dictionary<Guid, StoreManagement> EntitiesManagements = new Dictionary<Guid, StoreManagement>();

            Store store1 = createStoreObject("nike");
            Store store2 = createStoreObject("adidas");

            User u1 = createUserObject("Matan");
            User u2 = createUserObject("Benny");
            User u3 = createUserObject("Omer");
            User u4 = createUserObject("Ori");
            User u5 = createUserObject("Arik");
            
            StoreOwnership s11 = createStoreOwnershipObject(store1,u1);
            StoreOwnership s12 = createStoreOwnershipObject(store2,u1);

            StoreOwnership s2 = createStoreOwnershipObject(store2,u2);
            StoreManagement s3 = createStoreManagementObject(store2,u3);

            EntitiesOwnerships[s11.Guid]=s11;
            EntitiesOwnerships[s12.Guid]=s12;
            EntitiesOwnerships[s2.Guid]=s2;
            EntitiesManagements[s3.Guid] = s3;
                
            var repoOwnerships = DalMockFactory.MockRepository(EntitiesOwnerships);
            var repoManagements = DalMockFactory.MockRepository(EntitiesManagements);
            var us = new UsersService(mapper,loggerMock.Object,repoOwnerships.Object, repoManagements.Object );

            
            //FAIL:u2 is an owner of another store
            StoreOwnership s4 = createStoreOwnershipObject(store1,u4);
            var newOwner = mapper.Map<StoreOwnershipDto>(s4);
            var result2 = await us.NominateNewStoreOwner(u2.Guid,newOwner);
            result2.Should().BeFalse();

        }
        [Fact]
        public async void NominateNewStoreOwnerTest_ReturnFalse_ifUserThatIsntAnOwnerTriesToNominateNewOwner()
        {
            var mapper = config.CreateMapper();
            
            Dictionary<Guid, StoreOwnership> EntitiesOwnerships = new Dictionary<Guid, StoreOwnership>();
            Dictionary<Guid, StoreManagement> EntitiesManagements = new Dictionary<Guid, StoreManagement>();

            Store store1 = createStoreObject("nike");
            Store store2 = createStoreObject("adidas");

            User u1 = createUserObject("Matan");
            User u2 = createUserObject("Benny");
            User u3 = createUserObject("Omer");
            User u4 = createUserObject("Ori");
            User u5 = createUserObject("Arik");
            
            StoreOwnership s11 = createStoreOwnershipObject(store1,u1);
            StoreOwnership s12 = createStoreOwnershipObject(store2,u1);

            StoreOwnership s2 = createStoreOwnershipObject(store2,u2);
            StoreManagement s3 = createStoreManagementObject(store2,u3);

            EntitiesOwnerships[s11.Guid]=s11;
            EntitiesOwnerships[s12.Guid]=s12;
            EntitiesOwnerships[s2.Guid]=s2;
            EntitiesManagements[s3.Guid] = s3;
                
            var repoOwnerships = DalMockFactory.MockRepository(EntitiesOwnerships);
            var repoManagements = DalMockFactory.MockRepository(EntitiesManagements);
            var us = new UsersService(mapper,loggerMock.Object,repoOwnerships.Object, repoManagements.Object );

            
            StoreOwnership s4 = createStoreOwnershipObject(store1,u4);
            var newOwner = mapper.Map<StoreOwnershipDto>(s4);
            var result3 = await us.NominateNewStoreOwner(u5.Guid,newOwner);//FAIL:u5 is not an owner wanted store
            result3.Should().BeFalse();
            

        }
        [Fact]
        public async void NominateNewStoreOwnerTest_ReturnFalse_ifOwnerTriesToNominateNewOwnerThatIsAlreadyAnOwnerOfThatStore()
        {
            var mapper = config.CreateMapper();
            
            Dictionary<Guid, StoreOwnership> EntitiesOwnerships = new Dictionary<Guid, StoreOwnership>();
            Dictionary<Guid, StoreManagement> EntitiesManagements = new Dictionary<Guid, StoreManagement>();

            Store store1 = createStoreObject("nike");
            Store store2 = createStoreObject("adidas");

            User u1 = createUserObject("Matan");
            User u2 = createUserObject("Benny");
            User u3 = createUserObject("Omer");
            User u4 = createUserObject("Ori");
            User u5 = createUserObject("Arik");
            
            StoreOwnership s11 = createStoreOwnershipObject(store1,u1);
            StoreOwnership s12 = createStoreOwnershipObject(store2,u1);

            StoreOwnership s2 = createStoreOwnershipObject(store2,u2);
            StoreManagement s3 = createStoreManagementObject(store2,u3);

            EntitiesOwnerships[s11.Guid]=s11;
            EntitiesOwnerships[s12.Guid]=s12;
            EntitiesOwnerships[s2.Guid]=s2;
            EntitiesManagements[s3.Guid] = s3;
                
            var repoOwnerships = DalMockFactory.MockRepository(EntitiesOwnerships);
            var repoManagements = DalMockFactory.MockRepository(EntitiesManagements);
            var us = new UsersService(mapper,loggerMock.Object,repoOwnerships.Object, repoManagements.Object );
            
            
            var newOwner = mapper.Map<StoreOwnershipDto>(s2);
            var result4 = await us.NominateNewStoreOwner(u1.Guid,newOwner);//Fail : both are owners
            result4.Should().BeFalse();
            
          

        }
        [Fact]
        public async void NominateNewStoreOwnerTest_ReturnFalse_ifOwnerTriesToNominateNewOwnerThatIsAlreadyAStoreManagerOfThatStore()
        {
            var mapper = config.CreateMapper();
            
            Dictionary<Guid, StoreOwnership> EntitiesOwnerships = new Dictionary<Guid, StoreOwnership>();
            Dictionary<Guid, StoreManagement> EntitiesManagements = new Dictionary<Guid, StoreManagement>();

            Store store1 = createStoreObject("nike");
            Store store2 = createStoreObject("adidas");

            User u1 = createUserObject("Matan");
            User u2 = createUserObject("Benny");
            User u3 = createUserObject("Omer");
            User u4 = createUserObject("Ori");
            User u5 = createUserObject("Arik");
            
            StoreOwnership s11 = createStoreOwnershipObject(store1,u1);
            StoreOwnership s12 = createStoreOwnershipObject(store2,u1);

            StoreOwnership s2 = createStoreOwnershipObject(store2,u2);
            StoreManagement s3 = createStoreManagementObject(store2,u3);

            EntitiesOwnerships[s11.Guid]=s11;
            EntitiesOwnerships[s12.Guid]=s12;
            EntitiesOwnerships[s2.Guid]=s2;
            EntitiesManagements[s3.Guid] = s3;
                
            var repoOwnerships = DalMockFactory.MockRepository(EntitiesOwnerships);
            var repoManagements = DalMockFactory.MockRepository(EntitiesManagements);
            var us = new UsersService(mapper,loggerMock.Object,repoOwnerships.Object, repoManagements.Object );

          
            
            StoreOwnership s32 = createStoreOwnershipObject(store2,u3);
            var newOwner = mapper.Map<StoreOwnershipDto>(s32);
            var result5 = await us.NominateNewStoreOwner(u2.Guid,newOwner);//Fail : already a manager
            result5.Should().BeFalse();

        }
        
        
        [Fact]
        public async void NominateNewStoreManagerTest_ReturnTrue_WhenAnOwnerNominatesNewManagerThatDoesntHaveOtherResponsibility()
        {
            
            var mapper = config.CreateMapper();
            
            Dictionary<Guid, StoreOwnership> EntitiesOwnerships = new Dictionary<Guid, StoreOwnership>();
            Dictionary<Guid, StoreManagement> EntitiesManagements = new Dictionary<Guid, StoreManagement>();

            Store store1 = createStoreObject("nike");
            Store store2 = createStoreObject("adidas");

            User u1 = createUserObject("Matan");
            User u2 = createUserObject("Benny");
            User u3 = createUserObject("Omer");
            User u4 = createUserObject("Ori");
            User u5 = createUserObject("Arik");
            
            StoreOwnership s11 = createStoreOwnershipObject(store1,u1);
            StoreOwnership s12 = createStoreOwnershipObject(store2,u1);

            StoreOwnership s2 = createStoreOwnershipObject(store2,u2);
            StoreManagement s3 = createStoreManagementObject(store2,u3);

            EntitiesOwnerships[s11.Guid]=s11;
            EntitiesOwnerships[s12.Guid]=s12;
            EntitiesOwnerships[s2.Guid]=s2;
            EntitiesManagements[s3.Guid] = s3;
                
            var repoOwnerships = DalMockFactory.MockRepository(EntitiesOwnerships);
            var repoManagements = DalMockFactory.MockRepository(EntitiesManagements);
            var us = new UsersService(mapper,loggerMock.Object,repoOwnerships.Object, repoManagements.Object );

            
            //SUCCESS
            StoreManagement s22 = createStoreManagementObject(store1,u2);
            var newManager = mapper.Map<StoreManagementDto>(s22);
            var result1 = await us.NominateNewStoreManager(u1.Guid,newManager);
            result1.Should().BeTrue();
            
        }
        [Fact]
        public async void NominateNewStoreManagerTest_ReturnFalse_ifOwnerOfAStoreTriesToNominateNewManagerToAnotherStore()
        {
            
            var mapper = config.CreateMapper();
            
            Dictionary<Guid, StoreOwnership> EntitiesOwnerships = new Dictionary<Guid, StoreOwnership>();
            Dictionary<Guid, StoreManagement> EntitiesManagements = new Dictionary<Guid, StoreManagement>();

            Store store1 = createStoreObject("nike");
            Store store2 = createStoreObject("adidas");

            User u1 = createUserObject("Matan");
            User u2 = createUserObject("Benny");
            User u3 = createUserObject("Omer");
            User u4 = createUserObject("Ori");
            User u5 = createUserObject("Arik");
            
            StoreOwnership s11 = createStoreOwnershipObject(store1,u1);
            StoreOwnership s12 = createStoreOwnershipObject(store2,u1);

            StoreOwnership s2 = createStoreOwnershipObject(store2,u2);
            StoreManagement s3 = createStoreManagementObject(store2,u3);

            EntitiesOwnerships[s11.Guid]=s11;
            EntitiesOwnerships[s12.Guid]=s12;
            EntitiesOwnerships[s2.Guid]=s2;
            EntitiesManagements[s3.Guid] = s3;
                
            var repoOwnerships = DalMockFactory.MockRepository(EntitiesOwnerships);
            var repoManagements = DalMockFactory.MockRepository(EntitiesManagements);
            var us = new UsersService(mapper,loggerMock.Object,repoOwnerships.Object, repoManagements.Object );

            
            //FAIL:u2 is an owner of another store
            StoreManagement s4 = createStoreManagementObject(store1,u4);
            var newManager = mapper.Map<StoreManagementDto>(s4);
            var result2 = await us.NominateNewStoreManager(u2.Guid,newManager);
            result2.Should().BeFalse();
            
           
        }
        [Fact]
        public async void NominateNewStoreManagerTest_ReturnFalse_ifUserThatIsntAnOwnerTriesToNominateNewManager()
        {
            
            var mapper = config.CreateMapper();
            
            Dictionary<Guid, StoreOwnership> EntitiesOwnerships = new Dictionary<Guid, StoreOwnership>();
            Dictionary<Guid, StoreManagement> EntitiesManagements = new Dictionary<Guid, StoreManagement>();

            Store store1 = createStoreObject("nike");
            Store store2 = createStoreObject("adidas");

            User u1 = createUserObject("Matan");
            User u2 = createUserObject("Benny");
            User u3 = createUserObject("Omer");
            User u4 = createUserObject("Ori");
            User u5 = createUserObject("Arik");
            
            StoreOwnership s11 = createStoreOwnershipObject(store1,u1);
            StoreOwnership s12 = createStoreOwnershipObject(store2,u1);

            StoreOwnership s2 = createStoreOwnershipObject(store2,u2);
            StoreManagement s3 = createStoreManagementObject(store2,u3);

            EntitiesOwnerships[s11.Guid]=s11;
            EntitiesOwnerships[s12.Guid]=s12;
            EntitiesOwnerships[s2.Guid]=s2;
            EntitiesManagements[s3.Guid] = s3;
                
            var repoOwnerships = DalMockFactory.MockRepository(EntitiesOwnerships);
            var repoManagements = DalMockFactory.MockRepository(EntitiesManagements);
            var us = new UsersService(mapper,loggerMock.Object,repoOwnerships.Object, repoManagements.Object );
            
            
            StoreManagement s41 = createStoreManagementObject(store1,u4);
            var newManager = mapper.Map<StoreManagementDto>(s41);
            var result3 = await us.NominateNewStoreManager(u5.Guid,newManager);//FAIL:u5 is not an owner wanted store
            result3.Should().BeFalse();
            
          
        }
        [Fact]
        public async void NominateNewStoreManagerTest_ReturnFalse_ifOwnerTriesToNominateNewManagerThatIsAlreadyAnOwnerOfThatStore()
        {
            
            var mapper = config.CreateMapper();
            
            Dictionary<Guid, StoreOwnership> EntitiesOwnerships = new Dictionary<Guid, StoreOwnership>();
            Dictionary<Guid, StoreManagement> EntitiesManagements = new Dictionary<Guid, StoreManagement>();

            Store store1 = createStoreObject("nike");
            Store store2 = createStoreObject("adidas");

            User u1 = createUserObject("Matan");
            User u2 = createUserObject("Benny");
            User u3 = createUserObject("Omer");
            User u4 = createUserObject("Ori");
            User u5 = createUserObject("Arik");
            
            StoreOwnership s11 = createStoreOwnershipObject(store1,u1);
            StoreOwnership s12 = createStoreOwnershipObject(store2,u1);

            StoreOwnership s2 = createStoreOwnershipObject(store1,u2);
            StoreManagement s3 = createStoreManagementObject(store2,u3);

            EntitiesOwnerships[s11.Guid]=s11;
            EntitiesOwnerships[s12.Guid]=s12;
            EntitiesOwnerships[s2.Guid]=s2;
            EntitiesManagements[s3.Guid] = s3;
                
            var repoOwnerships = DalMockFactory.MockRepository(EntitiesOwnerships);
            var repoManagements = DalMockFactory.MockRepository(EntitiesManagements);
            var us = new UsersService(mapper,loggerMock.Object,repoOwnerships.Object, repoManagements.Object );

          
            
            StoreManagement m22 = createStoreManagementObject(store1,u2);
            var newManager = mapper.Map<StoreManagementDto>(m22);
            var result4 = await us.NominateNewStoreManager(u1.Guid,newManager);//Fail : both are owners
            result4.Should().BeFalse();
            
           
        }
        [Fact]
        public async void NominateNewStoreManagerTest_ReturnFalse_ifOwnerTriesToNominateNewManagerThatIsAlreadyAStoreManagerOfThatStore()
        {
            
            var mapper = config.CreateMapper();
            
            Dictionary<Guid, StoreOwnership> EntitiesOwnerships = new Dictionary<Guid, StoreOwnership>();
            Dictionary<Guid, StoreManagement> EntitiesManagements = new Dictionary<Guid, StoreManagement>();

            Store store1 = createStoreObject("nike");
            Store store2 = createStoreObject("adidas");

            User u1 = createUserObject("Matan");
            User u2 = createUserObject("Benny");
            User u3 = createUserObject("Omer");
            User u4 = createUserObject("Ori");
            User u5 = createUserObject("Arik");
            
            StoreOwnership s11 = createStoreOwnershipObject(store1,u1);
            StoreOwnership s12 = createStoreOwnershipObject(store2,u1);

            StoreOwnership s2 = createStoreOwnershipObject(store2,u2);
            StoreManagement s3 = createStoreManagementObject(store2,u3);

            EntitiesOwnerships[s11.Guid]=s11;
            EntitiesOwnerships[s12.Guid]=s12;
            EntitiesOwnerships[s2.Guid]=s2;
            EntitiesManagements[s3.Guid] = s3;
                
            var repoOwnerships = DalMockFactory.MockRepository(EntitiesOwnerships);
            var repoManagements = DalMockFactory.MockRepository(EntitiesManagements);
            var us = new UsersService(mapper,loggerMock.Object,repoOwnerships.Object, repoManagements.Object );
            
            
            StoreManagement s32 = createStoreManagementObject(store2,u3);
            var newManager = mapper.Map<StoreManagementDto>(s32);
            var result5 = await us.NominateNewStoreManager(u2.Guid,newManager);//Fail : already a manager
            result5.Should().BeFalse();
        }


        private Store createStoreObject(string storeName)
        {
            return new (){StoreName = storeName};
        }
        
        private User createUserObject(string name)
        {
            return new (){Name = name};
        }
        
        private StoreOwnership createStoreOwnershipObject(Store store , User user)
        {
         return new StoreOwnership(){Store = store,User = user};
   
        }
        
        private StoreManagement createStoreManagementObject(Store store , User user)
        {
            return new StoreManagement(){Store = store,User = user};

        }
        

    }
}