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
            var pr1 = new PurchaseProduct();
            var pr2 = new PurchaseProduct();
            var pr3 = new PurchaseProduct();

            var prList1 = new List<PurchaseProduct>();
            prList1.Add(pr1);
            prList1.Add(pr2);
            prList1.Add(pr3);

            StorePurchase p1 = new() {PurchaseProducts = prList1 , Store = s1};
               
                //p2
            var pr4 = new PurchaseProduct();
            var pr5 = new PurchaseProduct(); 

            var prList2 = new List<PurchaseProduct>();
            prList2.Add(pr4);
            prList2.Add(pr5);
            

            StorePurchase p2 = new() { PurchaseProducts = prList2, Store = s1 };

            //adidasStore purchase 
            var pr6 = new PurchaseProduct();
            var prList3 = new List<PurchaseProduct>();
            prList3.Add(pr6);

            StorePurchase p3 = new() { PurchaseProducts = prList3, Store = storeOwnershipBennyAdidas };

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
            
            //Assert
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

        [Fact]
        public async void GetSellersInformation_ShouldReturnStoreSellers_WhenStoreExists()
        {
            // Arrange 
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


            var uow = DalMockFactory.MockStoreUnitOfWork(null, entitiesStoreOwnerships, null, entitiesStoreManagements, null);


            StoresService s = new(loggerMock.Object, mapper, uow.Object);

            List<StoreManagementDto> lsm = new List<StoreManagementDto>
            {
                mapper.Map<StoreManagementDto>(smBenny),
                mapper.Map<StoreManagementDto>(smOmer)
            };

            List<StoreOwnershipDto> lso = new List<StoreOwnershipDto>
            {
                mapper.Map<StoreOwnershipDto>(soMatan),
                mapper.Map<StoreOwnershipDto>(soArye)
            };

            var expectedResponse = new StoreSellersResponse
            {
                StoreManagers = lsm,
                StoreOwners = lso
            };

            // Act 
            var response = await s.GetAllSellersInformation(storeNike.Guid);

            // Assert
            response.Should().BeEquivalentTo(expectedResponse);
        }

        [Fact]
        public async void GetSellersInformation_ShouldReturnEmptyObject_WhenStoreDoesNotExist()
        {
            // Arrange 
            Dictionary<Guid, StoreManagement> entitiesStoreManagements = new Dictionary<Guid, StoreManagement>();
            Dictionary<Guid, StoreOwnership> entitiesStoreOwnerships = new Dictionary<Guid, StoreOwnership>();

            var repoOwnerships = DalMockFactory.MockRepository(entitiesStoreOwnerships);

            var repoManagements = DalMockFactory.MockRepository(entitiesStoreManagements);

            var uow = DalMockFactory.MockStoreUnitOfWork(null, entitiesStoreOwnerships, null, entitiesStoreManagements, null);


            StoresService s = new(loggerMock.Object, mapper, uow.Object);

            // Act 
            var response = await s.GetAllSellersInformation(new Guid()); // Store Guid does not exist !!

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
            


            Store s1 = GetStoreData("Adidas");

            User u1 = GetUserData("Benny", "Skidanov", "BennySkidanov");
            User u2 = GetUserData("Omer", "Kempner", "OmerKempner");

            StoreManagement sm1 = GetStoreManagementData(u1, s1);
            StoreManagement sm2 = GetStoreManagementData(u1, s1);

            StoreManagementPermission smp1 = GetStoreManagementPermissionData(true, sm1);
            StoreManagementPermission smp2 = GetStoreManagementPermissionData(false, sm2);

            entitiesStoreManagementPermissions[sm1.Guid] = smp1;
            entitiesStoreManagementPermissions[sm2.Guid] = smp2;


           

            var uow = DalMockFactory.MockStoreUnitOfWork(null, null, null, null, entitiesStoreManagementPermissions);


            StoresService s = new(loggerMock.Object, mapper, uow.Object);
            

            // Act 
            var res1 = await s.GetPermissions(smp1.Guid);
            var res2 = await s.GetPermissions(smp2.Guid);

            var r1 = mapper.Map<StoreManagementPermission>(res1);
            var r2 = mapper.Map<StoreManagementPermission>(res2);

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


            var uow = DalMockFactory.MockStoreUnitOfWork(null, null, null, null, entitiesStoreManagementPermissions);


            StoresService s = new(loggerMock.Object, mapper, uow.Object);

            // Act 
            var res1 = await s.GetPermissions(new Guid());

            var r1 = mapper.Map<StoreManagementPermission>(res1);

            // Assert
            r1.Should().BeNull();
        }

        [Fact]
        public async void UpdatePermissions_UpdatePermissionsCorrectly_WhenStoreManagerDtoExist()
        {
            // Arrange
            Dictionary<Guid, StoreManagementPermission> entitiesStoreManagementPermissions =
                new Dictionary<Guid, StoreManagementPermission>();

            Store s1 = GetStoreData("MaccabiTelAvivFanStore");

            User u1 = GetUserData("Benny", "Skidanov", "BennySkidanov");

            StoreManagement sm1 = GetStoreManagementData(u1, s1);

            StoreManagementPermission smp1 = GetStoreManagementPermissionData(true, sm1);

            entitiesStoreManagementPermissions[smp1.Guid] = smp1;

            var repoPermissions = DalMockFactory.MockRepository(entitiesStoreManagementPermissions);

            var uow = DalMockFactory.MockStoreUnitOfWork(null, null, null, null, entitiesStoreManagementPermissions);


            StoresService s = new(loggerMock.Object, mapper, uow.Object);


            // Act 
            var replace1 = await s.GetPermissions(smp1.Guid);
            replace1.CanDoSomething = false;

            await s.UpdatePermission(replace1);

            var res1 = await s.GetPermissions(smp1.Guid);

            var r1 = mapper.Map<StoreManagementPermission>(res1);

            // Assert
            r1.CanDoSomething.Should().BeFalse();
        }

        [Fact]
        public async void UpdatePermissions_UpdatePermissionNotUpdated_WhenSMDoesNotExist()
        {
            // Arrange
            Dictionary<Guid, StoreManagementPermission> entitiesStoreManagementPermissions =
                new Dictionary<Guid, StoreManagementPermission>();
            

            Store s1 = GetStoreData("MaccabiTelAvivFanStore");

            User u1 = GetUserData("Benny", "Skidanov", "BennySkidanov");

            StoreManagement sm1 = GetStoreManagementData(u1, s1);

            StoreManagementPermission smp1 = GetStoreManagementPermissionData(true, sm1);

            var uow = DalMockFactory.MockStoreUnitOfWork(null, null, null, null, entitiesStoreManagementPermissions);

            entitiesStoreManagementPermissions[smp1.Guid] = smp1;
            StoresService s = new(loggerMock.Object, mapper, uow.Object);


            // Act 
            var replace1 = await s.GetPermissions(smp1.Guid);
            replace1.CanDoSomething = false;

            await s.UpdatePermission(new StoreManagementPermissionDto());

            var res1 = await s.GetPermissions(smp1.Guid);

            var r1 = mapper.Map<StoreManagementPermission>(res1);

            // Assert 
            r1.CanDoSomething.Should().BeTrue();
        }

        private static User GetUserData(string fName, string lName, string uname)
        {
            return new User() { Name = fName, LastName = lName, UserName = uname };
        }

        private static Store GetStoreData(string name)
        {
            return new Store() { StoreName = name };
        }

        private static StoreManagement GetStoreManagementData(User u, Store s)
        {
            return new StoreManagement() { User = u, Store = s };
        }

        private static StoreOwnership GetStoreOwnershipData(User u, Store s)
        {
            return new StoreOwnership() { User = u, Store = s };
        }

        private static StoreManagementPermission GetStoreManagementPermissionData(bool flag, StoreManagement sm)
        {
            return new StoreManagementPermission() { CanDoSomething = flag, StoreManagement = sm };
        }


    }
}