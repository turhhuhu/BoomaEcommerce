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
            Store storeOwnershipBennyAdidas = new() { StoreName = "Nike" };
            Store storeManagementsOmerAdidas = new() { StoreName = "Adidas" };
            Store s4 = new() { StoreName = "TopShop" };

            _EntitiesStores.Add(s1.Guid, s1);
            _EntitiesStores.Add(storeOwnershipBennyAdidas.Guid, storeOwnershipBennyAdidas);
            _EntitiesStores.Add(storeManagementsOmerAdidas.Guid, storeManagementsOmerAdidas);

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
            expectedRes3.Add(mapper.Map<StoreDto>(storeOwnershipBennyAdidas));
            expectedRes3.Add(mapper.Map<StoreDto>(storeManagementsOmerAdidas));
            res3.ToList().Should().BeEquivalentTo(expectedRes3);
        }

        [Fact]
        public async void GetStorePurchaseHistoryTest()
        {
            Mock<ILogger<StoresService>> loggerMock = new Mock<ILogger<StoresService>>();
            Store s1 = new() {};
            Store storeOwnershipBennyAdidas = new() {};
            Store storeManagementsOmerAdidas = new() {};

            //nikeStore purchase 
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

            //adidasStore purchase 
            var pr6 = _fixture.Build<PurchaseProduct>().Create();
            var prList3 = new List<PurchaseProduct>();
            prList3.Add(pr6);

            StorePurchase p3 = new() { ProductsPurchases = prList3, Store = storeOwnershipBennyAdidas };

            _EntitiesStorePurchases.Add(p1.Guid, p1);
            _EntitiesStorePurchases.Add(p2.Guid, p2);
            _EntitiesStorePurchases.Add(p3.Guid, p3);

            _EntitiesStores.Add(s1.Guid,s1);
            _EntitiesStores.Add(storeOwnershipBennyAdidas.Guid, storeOwnershipBennyAdidas);

            var storeUnitOfWork =
                DalMockFactory.MockStoreUnitOfWork(_EntitiesStores, null, _EntitiesStorePurchases, null, null);

            var storesService = new StoresService(loggerMock.Object, mapper, storeUnitOfWork.Object);

            var res1 = await storesService.GetStorePurchaseHistory(s1.Guid);
            var expectedRes1 = new List<StorePurchaseDto>();
            expectedRes1.Add(mapper.Map<StorePurchaseDto>(p1));
            expectedRes1.Add(mapper.Map<StorePurchaseDto>(p2));

            res1.ToList().Should().BeEquivalentTo(expectedRes1);

            var res2 = await storesService.GetStorePurchaseHistory(storeOwnershipBennyAdidas.Guid);
            var expectedRes2 = new List<StorePurchaseDto>();
            expectedRes2.Add(mapper.Map<StorePurchaseDto>(p3));

            res2.ToList().Should().BeEquivalentTo(expectedRes2);

            var res3 = await storesService.GetStorePurchaseHistory(storeManagementsOmerAdidas.Guid);
            res3.Should().BeEmpty();


        }

        [Fact]
        public async void NominateNewStoreOwner_ReturnTrue_NewOwnerDoesNotHaveOtherResponsibilities()
        {
            // Arrange
            var mapper = config.CreateMapper();

            var EntitiesOwnerships = new Dictionary<Guid, StoreOwnership>();
            var EntitiesManagements = new Dictionary<Guid, StoreManagement>();

            var nikeStore = createStoreObject("nike");

            var matanUser = createUserObject("Matan");
            var bennyUser = createUserObject("Benny");
            
            var storeOwnershipMatanNike = createStoreOwnershipObject(nikeStore, matanUser);
            

            EntitiesOwnerships[storeOwnershipMatanNike.Guid] = storeOwnershipMatanNike;
            

            var storeUnitOfWork = DalMockFactory.MockStoreUnitOfWork(null, EntitiesOwnerships, null, EntitiesManagements, null);

            var us = new StoresService(loggerMock.Object, mapper, storeUnitOfWork.Object);


            //SUCCESS
            var storeOwnershipBennyNike = createStoreOwnershipObject(nikeStore, bennyUser);
            var newOwner = mapper.Map<StoreOwnershipDto>(storeOwnershipBennyNike);
            //Act
            var result = await us.NominateNewStoreOwner(matanUser.Guid, newOwner);
            
            //Assert
            result.Should().BeTrue();
            var returnedValue = await storeUnitOfWork.Object.StoreOwnershipRepo.FindOneAsync(x => x.Guid == storeOwnershipBennyNike.Guid);
            returnedValue.Should().NotBe(null);

        }
        [Fact]
        public async void NominateNewStoreOwner_ReturnFalse_OwnerOfAStoreTriesToNominateNewOwnerToAnotherStore()
        {
            // Arrange
            var mapper = config.CreateMapper();

            var EntitiesOwnerships = new Dictionary<Guid, StoreOwnership>();
            var EntitiesManagements = new Dictionary<Guid, StoreManagement>();

            var nikeStore = createStoreObject("nike");
            var adidasStore = createStoreObject("adidas");

            var bennyUser = createUserObject("Benny");
            var oriUser = createUserObject("Ori");

            
            StoreOwnership storeOwnershipBennyAdidas = createStoreOwnershipObject(adidasStore, bennyUser);
            
            EntitiesOwnerships[storeOwnershipBennyAdidas.Guid] = storeOwnershipBennyAdidas;
            
            var storeUnitOfWork = DalMockFactory.MockStoreUnitOfWork(null, EntitiesOwnerships, null, EntitiesManagements, null);

            var us = new StoresService(loggerMock.Object, mapper, storeUnitOfWork.Object);


            //FAIL:bennyUser is an owner of another store
            var storeOwnershipOriNike = createStoreOwnershipObject(nikeStore, oriUser);
            var newOwner = mapper.Map<StoreOwnershipDto>(storeOwnershipOriNike);
            //Act
            var result = await us.NominateNewStoreOwner(bennyUser.Guid, newOwner);
            
            //Assert
            result.Should().BeFalse();
            var returnedValue = await storeUnitOfWork.Object.StoreOwnershipRepo.FindOneAsync(x => x.Guid == storeOwnershipOriNike.Guid);
            returnedValue.Should().Be(null);

        }
        [Fact]
        public async void NominateNewStoreOwner_ReturnFalse_UserThatIsNotAnOwnerTriesToNominate()
        {
            // Arrange
            var mapper = config.CreateMapper();

            var EntitiesOwnerships = new Dictionary<Guid, StoreOwnership>();
            var EntitiesManagements = new Dictionary<Guid, StoreManagement>();

            var nikeStore = createStoreObject("nike");
           
            var oriUser = createUserObject("Ori");
            var arikUser = createUserObject("Arik");

            var storeUnitOfWork = DalMockFactory.MockStoreUnitOfWork(null, EntitiesOwnerships, null, EntitiesManagements, null);

            var us = new StoresService(loggerMock.Object, mapper, storeUnitOfWork.Object);


            var storeOwnershipOriNike = createStoreOwnershipObject(nikeStore, oriUser);
            var newOwner = mapper.Map<StoreOwnershipDto>(storeOwnershipOriNike);
            
            //Act
            var result = await us.NominateNewStoreOwner(arikUser.Guid, newOwner);//FAIL:arikUser is not an owner wanted store
            
            //Assert
            result.Should().BeFalse();
            var returnedValue = await storeUnitOfWork.Object.StoreOwnershipRepo.FindOneAsync(x => x.Guid == storeOwnershipOriNike.Guid);
            returnedValue.Should().Be(null);



        }
        [Fact]
        public async void NominateNewStoreOwner_ReturnFalse_OwnerTriesToNominateOtherOwner()
        {
            // Arrange
            var mapper = config.CreateMapper();

            var EntitiesOwnerships = new Dictionary<Guid, StoreOwnership>();
            var EntitiesManagements = new Dictionary<Guid, StoreManagement>();

            var adidasStore = createStoreObject("adidas");

            var matanUser = createUserObject("Matan");
            var bennyUser = createUserObject("Benny");
          

            var storeOwnershipMatanAdidas = createStoreOwnershipObject(adidasStore, matanUser);

            var storeOwnershipBennyAdidas = createStoreOwnershipObject(adidasStore, bennyUser);

            EntitiesOwnerships[storeOwnershipMatanAdidas.Guid] = storeOwnershipMatanAdidas;
            EntitiesOwnerships[storeOwnershipBennyAdidas.Guid] = storeOwnershipBennyAdidas;


            var storeUnitOfWork = DalMockFactory.MockStoreUnitOfWork(null, EntitiesOwnerships, null, EntitiesManagements, null);

            var us = new StoresService(loggerMock.Object, mapper, storeUnitOfWork.Object);
            
            var newOwner = mapper.Map<StoreOwnershipDto>(storeOwnershipBennyAdidas);
            
            //Act
            var result = await us.NominateNewStoreOwner(matanUser.Guid, newOwner);//Fail : both are owners
            //Assert
            result.Should().BeFalse();
            
            

            

        }
        [Fact]
        public async void NominateNewStoreOwner_ReturnFalse_OwnerTriesToNominateOtherStoreManager()
        {
            // Arrange
            var mapper = config.CreateMapper();

            var EntitiesOwnerships = new Dictionary<Guid, StoreOwnership>();
            var EntitiesManagements = new Dictionary<Guid, StoreManagement>();

            var adidasStore = createStoreObject("adidas");

            var bennyUser = createUserObject("Benny");
            var omerUser = createUserObject("Omer");
           
            

            var storeOwnershipBennyAdidas = createStoreOwnershipObject(adidasStore, bennyUser);
            var storeManagementsOmerAdidas = createStoreManagementObject(adidasStore, omerUser);

           
            EntitiesOwnerships[storeOwnershipBennyAdidas.Guid] = storeOwnershipBennyAdidas;
            EntitiesManagements[storeManagementsOmerAdidas.Guid] = storeManagementsOmerAdidas;


            var storeUnitOfWork = DalMockFactory.MockStoreUnitOfWork(null, EntitiesOwnerships, null, EntitiesManagements, null);

            var us = new StoresService(loggerMock.Object, mapper, storeUnitOfWork.Object);
            
            var storeOwnershipOmerAdidas = createStoreOwnershipObject(adidasStore, omerUser);
            var newOwner = mapper.Map<StoreOwnershipDto>(storeOwnershipOmerAdidas);
            //Act
            var result = await us.NominateNewStoreOwner(bennyUser.Guid, newOwner);//Fail : already a manager
            
            //Assert
            result.Should().BeFalse();

            var returnedValue = await storeUnitOfWork.Object.StoreOwnershipRepo.FindOneAsync(x => x.Guid == storeOwnershipOmerAdidas.Guid);
            returnedValue.Should().Be(null);

        }


        [Fact]
        public async void NominateNewStoreManager_ReturnTrue_NewManagerDoesNotHaveOtherResponsibilities()
        {
            // Arrange
            var mapper = config.CreateMapper();

            var EntitiesOwnerships = new Dictionary<Guid, StoreOwnership>();
            var EntitiesManagements = new Dictionary<Guid, StoreManagement>();

            var nikeStore = createStoreObject("nike");
            

            var matanUser = createUserObject("Matan");
            var bennyUser = createUserObject("Benny");
           

            var storeOwnershipMatanNike = createStoreOwnershipObject(nikeStore, matanUser);
           

            EntitiesOwnerships[storeOwnershipMatanNike.Guid] = storeOwnershipMatanNike;


            var storeUnitOfWork = DalMockFactory.MockStoreUnitOfWork(null, EntitiesOwnerships, null, EntitiesManagements, null);

            var us = new StoresService(loggerMock.Object, mapper, storeUnitOfWork.Object);


            //SUCCESS
            var storeManagementBennyNike = createStoreManagementObject(nikeStore, bennyUser);
            var newManager = mapper.Map<StoreManagementDto>(storeManagementBennyNike);
            
            //Act
            var result = await us.NominateNewStoreManager(matanUser.Guid, newManager);
            
            //Assert
            result.Should().BeTrue();
            var returnedValue = await storeUnitOfWork.Object.StoreManagementRepo.FindOneAsync(x => x.Guid == storeManagementBennyNike.Guid);
            returnedValue.Should().NotBe(null);


        }
        [Fact]
        public async void NominateNewStoreManager_ReturnFalse_OwnerTriesToNominateNewManagerToAnotherStore()
        {
            // Arrange
            var mapper = config.CreateMapper();

            var EntitiesOwnerships = new Dictionary<Guid, StoreOwnership>();
            var EntitiesManagements = new Dictionary<Guid, StoreManagement>();

            var nikeStore = createStoreObject("nike");
            var adidasStore = createStoreObject("adidas");

            var bennyUser = createUserObject("Benny");
            var oriUser = createUserObject("Ori");

           
            var storeOwnershipBennyAdidas = createStoreOwnershipObject(adidasStore, bennyUser);
           
            EntitiesOwnerships[storeOwnershipBennyAdidas.Guid] = storeOwnershipBennyAdidas;

            var storeUnitOfWork = DalMockFactory.MockStoreUnitOfWork(null, EntitiesOwnerships, null, EntitiesManagements, null);

            var us = new StoresService(loggerMock.Object, mapper, storeUnitOfWork.Object);


            //FAIL:bennyUser is an owner of another store
            var storeManagementsOriNike = createStoreManagementObject(nikeStore, oriUser);
            var newManager = mapper.Map<StoreManagementDto>(storeManagementsOriNike);
            
            //Act
            var result = await us.NominateNewStoreManager(bennyUser.Guid, newManager);
            
            //Assert
            result.Should().BeFalse();
            var returnedValue = await storeUnitOfWork.Object.StoreManagementRepo.FindOneAsync(x => x.Guid == storeManagementsOriNike.Guid);
            returnedValue.Should().Be(null);


        }
        [Fact]
        public async void NominateNewStoreManager_ReturnFalse_UserThatINotAnOwnerTriesToNominate()
        {
            // Arrange
            var mapper = config.CreateMapper();

            var EntitiesOwnerships = new Dictionary<Guid, StoreOwnership>();
            var EntitiesManagements = new Dictionary<Guid, StoreManagement>();

            var nikeStore = createStoreObject("nike");
            
            var oriUser = createUserObject("Ori");
            var arikUser = createUserObject("Arik");


            var storeUnitOfWork = DalMockFactory.MockStoreUnitOfWork(null, EntitiesOwnerships, null, EntitiesManagements, null);

            var us = new StoresService(loggerMock.Object, mapper, storeUnitOfWork.Object);


            var storeManagementsOriNike = createStoreManagementObject(nikeStore, oriUser);
            var newManager = mapper.Map<StoreManagementDto>(storeManagementsOriNike);
            
            //Act
            var result = await us.NominateNewStoreManager(arikUser.Guid, newManager);//FAIL:arikUser is not an owner wanted store
            
            //Assert
            result.Should().BeFalse();
            var returnedValue = await storeUnitOfWork.Object.StoreManagementRepo.FindOneAsync(x => x.Guid == storeManagementsOriNike.Guid);
            returnedValue.Should().Be(null);

        }
        [Fact]
        public async void NominateNewStoreManager_ReturnFalse_OwnerTriesToNominateOtherOwner()
        {
            // Arrange
            var mapper = config.CreateMapper();

            var EntitiesOwnerships = new Dictionary<Guid, StoreOwnership>();
            var EntitiesManagements = new Dictionary<Guid, StoreManagement>();

            var nikeStore = createStoreObject("nike");

            var matanUser = createUserObject("Matan");
            var bennyUser = createUserObject("Benny");
           

            var storeOwnershipMatanNike = createStoreOwnershipObject(nikeStore, matanUser);

            var storeOwnershipBennyAdidas = createStoreOwnershipObject(nikeStore, bennyUser);

            EntitiesOwnerships[storeOwnershipMatanNike.Guid] = storeOwnershipMatanNike;
            EntitiesOwnerships[storeOwnershipBennyAdidas.Guid] = storeOwnershipBennyAdidas;

            var storeUnitOfWork = DalMockFactory.MockStoreUnitOfWork(null, EntitiesOwnerships, null, EntitiesManagements, null);

            var us = new StoresService(loggerMock.Object, mapper, storeUnitOfWork.Object);



            var storeManagementBennyNike = createStoreManagementObject(nikeStore, bennyUser);
            var newManager = mapper.Map<StoreManagementDto>(storeManagementBennyNike);
            
            //Act
            var result = await us.NominateNewStoreManager(matanUser.Guid, newManager);//Fail : both are owners
            
            //Asser
            result.Should().BeFalse();

            var returnedValue = await storeUnitOfWork.Object.StoreManagementRepo.FindOneAsync(x => x.Guid == storeManagementBennyNike.Guid);
            returnedValue.Should().Be(null);

        }
        [Fact]
        public async void NominateNewStoreManager_ReturnFalse_OwnerTriesToNominateOtherStoreManager()
        {
            // Arrange
            var mapper = config.CreateMapper();

            var EntitiesOwnerships = new Dictionary<Guid, StoreOwnership>();
            var EntitiesManagements = new Dictionary<Guid, StoreManagement>();

            var adidasStore = createStoreObject("adidas");

            var bennyUser = createUserObject("Benny");
            var omerUser = createUserObject("Omer");

           

            var storeOwnershipBennyAdidas = createStoreOwnershipObject(adidasStore, bennyUser);
            var storeManagementsOmerAdidas = createStoreManagementObject(adidasStore, omerUser);
            
            EntitiesOwnerships[storeOwnershipBennyAdidas.Guid] = storeOwnershipBennyAdidas;
            EntitiesManagements[storeManagementsOmerAdidas.Guid] = storeManagementsOmerAdidas;


            var storeUnitOfWork = DalMockFactory.MockStoreUnitOfWork(null, EntitiesOwnerships, null, EntitiesManagements, null);

            var us = new StoresService(loggerMock.Object, mapper, storeUnitOfWork.Object);


            var newManager = mapper.Map<StoreManagementDto>(storeManagementsOmerAdidas);
            
            //Act
            var result = await us.NominateNewStoreManager(bennyUser.Guid, newManager);//Fail : already a manager
            
            //Asser
            result.Should().BeFalse();
            
            
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