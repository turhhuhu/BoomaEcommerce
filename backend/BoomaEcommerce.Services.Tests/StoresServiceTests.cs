using System;
using System.Collections.Generic;
using System.Linq;
using AutoFixture;
using AutoMapper;
using BoomaEcommerce.Domain;
using BoomaEcommerce.Services.DTO;
using BoomaEcommerce.Services.MappingProfiles;
using BoomaEcommerce.Services.Stores;
using BoomaEcommerce.Services.Users;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace BoomaEcommerce.Services.Tests
{
    public class StoresServiceTests
    {
        Dictionary<Guid, Store> _EntitiesStores = new Dictionary<Guid, Store>();
        Dictionary<Guid, StorePurchase> _EntitiesStorePurchases = new Dictionary<Guid, StorePurchase>();
        public Mock<ILogger<StoresService>> loggerMock = new Mock<ILogger<StoresService>>();

        static MapperConfiguration config = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile(new DomainToDtoProfile());
            cfg.AddProfile(new DtoToDomainProfile());
        });
        IMapper mapper = config.CreateMapper();
        private IFixture _fixture = new Fixture();
       
        [Fact]
        public async void GetStoreTest()
        {
            Mock<ILogger<StoresService>> loggerMock = new Mock<ILogger<StoresService>>();

            Store s1 = new() { StoreName = "Benny Hadayag" };
            Store s2 = new() { StoreName = "Nike" };
            Store s3 = new() { StoreName = "Adidas" };
            Store s4 = new() { StoreName = "TopShop" };

            _EntitiesStores.Add(s1.Guid, s1);
            _EntitiesStores.Add(s2.Guid, s2);
            _EntitiesStores.Add(s3.Guid, s3);

            var storeUnitOfWork =
                DalMockFactory.MockStoreUnitOfWork(_EntitiesStores, null, _EntitiesStorePurchases, null, null);

            var storesService = new StoresService(loggerMock.Object, mapper, storeUnitOfWork.Object);

            var res1 = await storesService.GetStoreAsync(s1.Guid);
            var expectedRes1 = mapper.Map<StoreDto>(s1);
            res1.Should().BeEquivalentTo(expectedRes1);

            var res2 = await storesService.GetStoreAsync(s4.Guid);
            res2.Should().BeNull();

            var res3 = await storesService.GetStoresAsync();
            List<StoreDto> expectedRes3 = new List<StoreDto>();
            expectedRes3.Add(mapper.Map<StoreDto>(s1));
            expectedRes3.Add(mapper.Map<StoreDto>(s2));
            expectedRes3.Add(mapper.Map<StoreDto>(s3));
            res3.ToList().Should().BeEquivalentTo(expectedRes3);
        }

        [Fact]
        public async void GetStorePurchaseHistoryTest()
        {
            Mock<ILogger<StoresService>> loggerMock = new Mock<ILogger<StoresService>>();
            Store s1 = new() {};
            Store s2 = new() {};
            Store s3 = new() {};

            //store1 purchase 
                //p1 
            var pr1 = _fixture.Build<PurchaseProduct>().Create();
            var pr2 = _fixture.Build<PurchaseProduct>().Create();
            var pr3 = _fixture.Build<PurchaseProduct>().Create();

            var prList1 = new List<PurchaseProduct>();
            prList1.Add(pr1);
            prList1.Add(pr2);
            prList1.Add(pr3);

            StorePurchase p1 = new() {ProductsPurchases = prList1 , Store = s1};
               
                //p2
            var pr4 = _fixture.Build<PurchaseProduct>().Create();
            var pr5 = _fixture.Build<PurchaseProduct>().Create(); 

            var prList2 = new List<PurchaseProduct>();
            prList2.Add(pr4);
            prList2.Add(pr5);
            

            StorePurchase p2 = new() { ProductsPurchases = prList2, Store = s1 };

            //store2 purchase 
            var pr6 = _fixture.Build<PurchaseProduct>().Create();
            var prList3 = new List<PurchaseProduct>();
            prList3.Add(pr6);

            StorePurchase p3 = new() { ProductsPurchases = prList3, Store = s2 };

            _EntitiesStorePurchases.Add(p1.Guid, p1);
            _EntitiesStorePurchases.Add(p2.Guid, p2);
            _EntitiesStorePurchases.Add(p3.Guid, p3);

            _EntitiesStores.Add(s1.Guid,s1);
            _EntitiesStores.Add(s2.Guid, s2);

            var storeUnitOfWork =
                DalMockFactory.MockStoreUnitOfWork(_EntitiesStores, null, _EntitiesStorePurchases, null, null);

            var storesService = new StoresService(loggerMock.Object, mapper, storeUnitOfWork.Object);

            var res1 = await storesService.GetStorePurchaseHistory(s1.Guid);
            var expectedRes1 = new List<StorePurchaseDto>();
            expectedRes1.Add(mapper.Map<StorePurchaseDto>(p1));
            expectedRes1.Add(mapper.Map<StorePurchaseDto>(p2));

            res1.ToList().Should().BeEquivalentTo(expectedRes1);

            var res2 = await storesService.GetStorePurchaseHistory(s2.Guid);
            var expectedRes2 = new List<StorePurchaseDto>();
            expectedRes2.Add(mapper.Map<StorePurchaseDto>(p3));

            res2.ToList().Should().BeEquivalentTo(expectedRes2);

            var res3 = await storesService.GetStorePurchaseHistory(s3.Guid);
            res3.Should().BeEmpty();


        }

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

            StoreOwnership s11 = createStoreOwnershipObject(store1, u1);
            StoreOwnership s12 = createStoreOwnershipObject(store2, u1);

            StoreOwnership s2 = createStoreOwnershipObject(store2, u2);
            StoreManagement s3 = createStoreManagementObject(store2, u3);

            EntitiesOwnerships[s11.Guid] = s11;
            EntitiesOwnerships[s12.Guid] = s12;
            EntitiesOwnerships[s2.Guid] = s2;
            EntitiesManagements[s3.Guid] = s3;

            var storeUnitOfWork = DalMockFactory.MockStoreUnitOfWork(null, EntitiesOwnerships, null, EntitiesManagements, null);

            var us = new StoresService(loggerMock.Object, mapper, storeUnitOfWork.Object);


            //SUCCESS
            StoreOwnership s22 = createStoreOwnershipObject(store1, u2);
            var newOwner = mapper.Map<StoreOwnershipDto>(s22);
            var result1 = await us.NominateNewStoreOwner(u1.Guid, newOwner);
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

            StoreOwnership s11 = createStoreOwnershipObject(store1, u1);
            StoreOwnership s12 = createStoreOwnershipObject(store2, u1);

            StoreOwnership s2 = createStoreOwnershipObject(store2, u2);
            StoreManagement s3 = createStoreManagementObject(store2, u3);

            EntitiesOwnerships[s11.Guid] = s11;
            EntitiesOwnerships[s12.Guid] = s12;
            EntitiesOwnerships[s2.Guid] = s2;
            EntitiesManagements[s3.Guid] = s3;


            var storeUnitOfWork = DalMockFactory.MockStoreUnitOfWork(null, EntitiesOwnerships, null, EntitiesManagements, null);

            var us = new StoresService(loggerMock.Object, mapper, storeUnitOfWork.Object);


            //FAIL:u2 is an owner of another store
            StoreOwnership s4 = createStoreOwnershipObject(store1, u4);
            var newOwner = mapper.Map<StoreOwnershipDto>(s4);
            var result2 = await us.NominateNewStoreOwner(u2.Guid, newOwner);
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

            StoreOwnership s11 = createStoreOwnershipObject(store1, u1);
            StoreOwnership s12 = createStoreOwnershipObject(store2, u1);

            StoreOwnership s2 = createStoreOwnershipObject(store2, u2);
            StoreManagement s3 = createStoreManagementObject(store2, u3);

            EntitiesOwnerships[s11.Guid] = s11;
            EntitiesOwnerships[s12.Guid] = s12;
            EntitiesOwnerships[s2.Guid] = s2;
            EntitiesManagements[s3.Guid] = s3;


            var storeUnitOfWork = DalMockFactory.MockStoreUnitOfWork(null, EntitiesOwnerships, null, EntitiesManagements, null);

            var us = new StoresService(loggerMock.Object, mapper, storeUnitOfWork.Object);


            StoreOwnership s4 = createStoreOwnershipObject(store1, u4);
            var newOwner = mapper.Map<StoreOwnershipDto>(s4);
            var result3 = await us.NominateNewStoreOwner(u5.Guid, newOwner);//FAIL:u5 is not an owner wanted store
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

            StoreOwnership s11 = createStoreOwnershipObject(store1, u1);
            StoreOwnership s12 = createStoreOwnershipObject(store2, u1);

            StoreOwnership s2 = createStoreOwnershipObject(store2, u2);
            StoreManagement s3 = createStoreManagementObject(store2, u3);

            EntitiesOwnerships[s11.Guid] = s11;
            EntitiesOwnerships[s12.Guid] = s12;
            EntitiesOwnerships[s2.Guid] = s2;
            EntitiesManagements[s3.Guid] = s3;


            var storeUnitOfWork = DalMockFactory.MockStoreUnitOfWork(null, EntitiesOwnerships, null, EntitiesManagements, null);

            var us = new StoresService(loggerMock.Object, mapper, storeUnitOfWork.Object);


            var newOwner = mapper.Map<StoreOwnershipDto>(s2);
            var result4 = await us.NominateNewStoreOwner(u1.Guid, newOwner);//Fail : both are owners
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

            StoreOwnership s11 = createStoreOwnershipObject(store1, u1);
            StoreOwnership s12 = createStoreOwnershipObject(store2, u1);

            StoreOwnership s2 = createStoreOwnershipObject(store2, u2);
            StoreManagement s3 = createStoreManagementObject(store2, u3);

            EntitiesOwnerships[s11.Guid] = s11;
            EntitiesOwnerships[s12.Guid] = s12;
            EntitiesOwnerships[s2.Guid] = s2;
            EntitiesManagements[s3.Guid] = s3;


            var storeUnitOfWork = DalMockFactory.MockStoreUnitOfWork(null, EntitiesOwnerships, null, EntitiesManagements, null);

            var us = new StoresService(loggerMock.Object, mapper, storeUnitOfWork.Object);



            StoreOwnership s32 = createStoreOwnershipObject(store2, u3);
            var newOwner = mapper.Map<StoreOwnershipDto>(s32);
            var result5 = await us.NominateNewStoreOwner(u2.Guid, newOwner);//Fail : already a manager
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

            StoreOwnership s11 = createStoreOwnershipObject(store1, u1);
            StoreOwnership s12 = createStoreOwnershipObject(store2, u1);

            StoreOwnership s2 = createStoreOwnershipObject(store2, u2);
            StoreManagement s3 = createStoreManagementObject(store2, u3);

            EntitiesOwnerships[s11.Guid] = s11;
            EntitiesOwnerships[s12.Guid] = s12;
            EntitiesOwnerships[s2.Guid] = s2;
            EntitiesManagements[s3.Guid] = s3;


            var storeUnitOfWork = DalMockFactory.MockStoreUnitOfWork(null, EntitiesOwnerships, null, EntitiesManagements, null);

            var us = new StoresService(loggerMock.Object, mapper, storeUnitOfWork.Object);


            //SUCCESS
            StoreManagement s22 = createStoreManagementObject(store1, u2);
            var newManager = mapper.Map<StoreManagementDto>(s22);
            var result1 = await us.NominateNewStoreManager(u1.Guid, newManager);
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

            StoreOwnership s11 = createStoreOwnershipObject(store1, u1);
            StoreOwnership s12 = createStoreOwnershipObject(store2, u1);

            StoreOwnership s2 = createStoreOwnershipObject(store2, u2);
            StoreManagement s3 = createStoreManagementObject(store2, u3);

            EntitiesOwnerships[s11.Guid] = s11;
            EntitiesOwnerships[s12.Guid] = s12;
            EntitiesOwnerships[s2.Guid] = s2;
            EntitiesManagements[s3.Guid] = s3;


            var storeUnitOfWork = DalMockFactory.MockStoreUnitOfWork(null, EntitiesOwnerships, null, EntitiesManagements, null);

            var us = new StoresService(loggerMock.Object, mapper, storeUnitOfWork.Object);


            //FAIL:u2 is an owner of another store
            StoreManagement s4 = createStoreManagementObject(store1, u4);
            var newManager = mapper.Map<StoreManagementDto>(s4);
            var result2 = await us.NominateNewStoreManager(u2.Guid, newManager);
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

            StoreOwnership s11 = createStoreOwnershipObject(store1, u1);
            StoreOwnership s12 = createStoreOwnershipObject(store2, u1);

            StoreOwnership s2 = createStoreOwnershipObject(store2, u2);
            StoreManagement s3 = createStoreManagementObject(store2, u3);

            EntitiesOwnerships[s11.Guid] = s11;
            EntitiesOwnerships[s12.Guid] = s12;
            EntitiesOwnerships[s2.Guid] = s2;
            EntitiesManagements[s3.Guid] = s3;


            var storeUnitOfWork = DalMockFactory.MockStoreUnitOfWork(null, EntitiesOwnerships, null, EntitiesManagements, null);

            var us = new StoresService(loggerMock.Object, mapper, storeUnitOfWork.Object);


            StoreManagement s41 = createStoreManagementObject(store1, u4);
            var newManager = mapper.Map<StoreManagementDto>(s41);
            var result3 = await us.NominateNewStoreManager(u5.Guid, newManager);//FAIL:u5 is not an owner wanted store
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

            StoreOwnership s11 = createStoreOwnershipObject(store1, u1);
            StoreOwnership s12 = createStoreOwnershipObject(store2, u1);

            StoreOwnership s2 = createStoreOwnershipObject(store1, u2);
            StoreManagement s3 = createStoreManagementObject(store2, u3);

            EntitiesOwnerships[s11.Guid] = s11;
            EntitiesOwnerships[s12.Guid] = s12;
            EntitiesOwnerships[s2.Guid] = s2;
            EntitiesManagements[s3.Guid] = s3;

            var storeUnitOfWork = DalMockFactory.MockStoreUnitOfWork(null, EntitiesOwnerships, null, EntitiesManagements, null);

            var us = new StoresService(loggerMock.Object, mapper, storeUnitOfWork.Object);



            StoreManagement m22 = createStoreManagementObject(store1, u2);
            var newManager = mapper.Map<StoreManagementDto>(m22);
            var result4 = await us.NominateNewStoreManager(u1.Guid, newManager);//Fail : both are owners
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

            StoreOwnership s11 = createStoreOwnershipObject(store1, u1);
            StoreOwnership s12 = createStoreOwnershipObject(store2, u1);

            StoreOwnership s2 = createStoreOwnershipObject(store2, u2);
            StoreManagement s3 = createStoreManagementObject(store2, u3);

            EntitiesOwnerships[s11.Guid] = s11;
            EntitiesOwnerships[s12.Guid] = s12;
            EntitiesOwnerships[s2.Guid] = s2;
            EntitiesManagements[s3.Guid] = s3;


            var storeUnitOfWork = DalMockFactory.MockStoreUnitOfWork(null, EntitiesOwnerships, null, EntitiesManagements, null);

            var us = new StoresService(loggerMock.Object, mapper, storeUnitOfWork.Object);


            StoreManagement s32 = createStoreManagementObject(store2, u3);
            var newManager = mapper.Map<StoreManagementDto>(s32);
            var result5 = await us.NominateNewStoreManager(u2.Guid, newManager);//Fail : already a manager
            result5.Should().BeFalse();
        }
        private Store createStoreObject(string storeName)
        {
            return new() { StoreName = storeName };
        }

        private User createUserObject(string name)
        {
            return new() { Name = name };
        }

        private StoreOwnership createStoreOwnershipObject(Store store, User user)
        {
            return new StoreOwnership() { Store = store, User = user };

        }

        private StoreManagement createStoreManagementObject(Store store, User user)
        {
            return new StoreManagement() { Store = store, User = user };

        }
    }
}