using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using BoomaEcommerce.Domain;
using BoomaEcommerce.Services.DTO;
using BoomaEcommerce.Services.Stores;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace BoomaEcommerce.Services.Tests
{
    public class StoresServiceTests
    {
        private readonly Mock<ILogger<StoresService>> _loggerMock = new();
        private readonly IMapper _mapper = MapperFactory.GetMapper();

        private StoresService GetStoreService( 
            IDictionary<Guid, Store> stores,
            IDictionary<Guid, StoreOwnership> storeOwnerships,
            IDictionary<Guid, StorePurchase> storePurchases,
            IDictionary<Guid, StoreManagement> storeManagements,
            IDictionary<Guid, StoreManagementPermission> storeManagementPermissions,
            IDictionary<Guid, Product> products)
        {
            var storeUnitOfWork = DalMockFactory.MockStoreUnitOfWork(stores, storeOwnerships, storePurchases,
                storeManagements, storeManagementPermissions, products);
            
            return new StoresService(_loggerMock.Object, _mapper, storeUnitOfWork.Object);
        }

        [Fact]
        public async void NominateNewStoreOwner_ReturnTrue_NewOwnerDoesNotHaveOtherResponsibilities()
        {
            // Arrange
            var entitiesOwnerships = new Dictionary<Guid, StoreOwnership>();
            var entitiesManagements = new Dictionary<Guid, StoreManagement>();

            var nikeStore = TestData.CreateStoreObject("nike");

            var matanUser = TestData.CreateUserObject("Matan");
            var bennyUser = TestData.CreateUserObject("Benny");
            
            var storeOwnershipMatanNike = TestData.CreateStoreOwnershipObject(nikeStore, matanUser);
            

            entitiesOwnerships[storeOwnershipMatanNike.Guid] = storeOwnershipMatanNike;
            
            var us = GetStoreService(null, entitiesOwnerships, null, entitiesManagements, null, null);

            //SUCCESS
            var storeOwnershipBennyNike = TestData.CreateStoreOwnershipObject(nikeStore, bennyUser);
            var newOwner = _mapper.Map<StoreOwnershipDto>(storeOwnershipBennyNike);
            //Act
            var result = await us.NominateNewStoreOwner(matanUser.Guid, newOwner);
            
            //Assert
            result.Should().BeTrue();
            entitiesOwnerships.Should().ContainKey(storeOwnershipBennyNike.Guid);

        }
        [Fact]
        public async void NominateNewStoreOwner_ReturnFalse_OwnerOfAStoreTriesToNominateNewOwnerToAnotherStore()
        {
            // Arrange
            var entitiesOwnerships = new Dictionary<Guid, StoreOwnership>();
            var entitiesManagements = new Dictionary<Guid, StoreManagement>();

            var nikeStore = TestData.CreateStoreObject("nike");
            var adidasStore = TestData.CreateStoreObject("adidas");

            var bennyUser = TestData.CreateUserObject("Benny");
            var oriUser = TestData.CreateUserObject("Ori");

            
            StoreOwnership storeOwnershipBennyAdidas = TestData.CreateStoreOwnershipObject(adidasStore, bennyUser);
            
            entitiesOwnerships[storeOwnershipBennyAdidas.Guid] = storeOwnershipBennyAdidas;
            
            var us = GetStoreService(null, entitiesOwnerships, null, entitiesManagements, null, null);


            //FAIL:bennyUser is an owner of another store
            var storeOwnershipOriNike = TestData.CreateStoreOwnershipObject(nikeStore, oriUser);
            var newOwner = _mapper.Map<StoreOwnershipDto>(storeOwnershipOriNike);
            //Act
            var result = await us.NominateNewStoreOwner(bennyUser.Guid, newOwner);
            
            //Assert
            result.Should().BeFalse();
            entitiesOwnerships.Should().NotContainKey(storeOwnershipOriNike.Guid);

        }
        [Fact]
        public async void NominateNewStoreOwner_ReturnFalse_UserThatIsNotAnOwnerTriesToNominate()
        {
            // Arrange
            var entitiesOwnerships = new Dictionary<Guid, StoreOwnership>();
            var entitiesManagements = new Dictionary<Guid, StoreManagement>();

            var nikeStore = TestData.CreateStoreObject("nike");
           
            var oriUser = TestData.CreateUserObject("Ori");
            var arikUser = TestData.CreateUserObject("Arik");

            var us = GetStoreService(null, entitiesOwnerships, null, entitiesManagements, null, null);
            
            var storeOwnershipOriNike = TestData.CreateStoreOwnershipObject(nikeStore, oriUser);
            var newOwner = _mapper.Map<StoreOwnershipDto>(storeOwnershipOriNike);
            
            //Act
            var result = await us.NominateNewStoreOwner(arikUser.Guid, newOwner);//FAIL:arikUser is not an owner wanted store
            
            //Assert
            result.Should().BeFalse();
            entitiesOwnerships.Should().NotContainKey(storeOwnershipOriNike.Guid);



        }
        [Fact]
        public async void NominateNewStoreOwner_ReturnFalse_OwnerTriesToNominateOtherOwner()
        {
            // Arrange
            var entitiesOwnerships = new Dictionary<Guid, StoreOwnership>();
            var entitiesManagements = new Dictionary<Guid, StoreManagement>();

            var adidasStore = TestData.CreateStoreObject("adidas");

            var matanUser = TestData.CreateUserObject("Matan");
            var bennyUser = TestData.CreateUserObject("Benny");
          

            var storeOwnershipMatanAdidas = TestData.CreateStoreOwnershipObject(adidasStore, matanUser);

            var storeOwnershipBennyAdidas = TestData.CreateStoreOwnershipObject(adidasStore, bennyUser);

            entitiesOwnerships[storeOwnershipMatanAdidas.Guid] = storeOwnershipMatanAdidas;
            entitiesOwnerships[storeOwnershipBennyAdidas.Guid] = storeOwnershipBennyAdidas;

            var us = GetStoreService(null, entitiesOwnerships, null, entitiesManagements, null, null);

            var newOwner = _mapper.Map<StoreOwnershipDto>(storeOwnershipBennyAdidas);
            
            //Act
            var result = await us.NominateNewStoreOwner(matanUser.Guid, newOwner);//Fail : both are owners
            //Assert
            result.Should().BeFalse();
            
            

            

        }
        [Fact]
        public async void NominateNewStoreOwner_ReturnFalse_OwnerTriesToNominateOtherStoreManager()
        {
            // Arrange
            var entitiesOwnerships = new Dictionary<Guid, StoreOwnership>();
            var entitiesManagements = new Dictionary<Guid, StoreManagement>();

            var adidasStore = TestData.CreateStoreObject("adidas");

            var bennyUser = TestData.CreateUserObject("Benny");
            var omerUser = TestData.CreateUserObject("Omer");
           
            

            var storeOwnershipBennyAdidas = TestData.CreateStoreOwnershipObject(adidasStore, bennyUser);
            var storeManagementsOmerAdidas = TestData.CreateStoreManagementObject(adidasStore, omerUser);

           
            entitiesOwnerships[storeOwnershipBennyAdidas.Guid] = storeOwnershipBennyAdidas;
            entitiesManagements[storeManagementsOmerAdidas.Guid] = storeManagementsOmerAdidas;


            var us = GetStoreService(null, entitiesOwnerships, null, entitiesManagements, null, null);
            
            var storeOwnershipOmerAdidas = TestData.CreateStoreOwnershipObject(adidasStore, omerUser);
            var newOwner = _mapper.Map<StoreOwnershipDto>(storeOwnershipOmerAdidas);
            //Act
            var result = await us.NominateNewStoreOwner(bennyUser.Guid, newOwner);//Fail : already a manager
            
            //Assert
            result.Should().BeFalse();
            entitiesOwnerships.Should().NotContainKey(storeOwnershipOmerAdidas.Guid);

        }


        [Fact]
        public async void NominateNewStoreManager_ReturnTrue_NewManagerDoesNotHaveOtherResponsibilities()
        {
            // Arrange
            var entitiesOwnerships = new Dictionary<Guid, StoreOwnership>();
            var entitiesManagements = new Dictionary<Guid, StoreManagement>();

            var nikeStore = TestData.CreateStoreObject("nike");
            

            var matanUser = TestData.CreateUserObject("Matan");
            var bennyUser = TestData.CreateUserObject("Benny");
           

            var storeOwnershipMatanNike = TestData.CreateStoreOwnershipObject(nikeStore, matanUser);
           

            entitiesOwnerships[storeOwnershipMatanNike.Guid] = storeOwnershipMatanNike;


           var us = GetStoreService(null, entitiesOwnerships, null, entitiesManagements, null, null);


            //SUCCESS
            var storeManagementBennyNike = TestData.CreateStoreManagementObject(nikeStore, bennyUser);
            var newManager = _mapper.Map<StoreManagementDto>(storeManagementBennyNike);
            
            //Act
            var result = await us.NominateNewStoreManager(matanUser.Guid, newManager);
            
            //Assert
            result.Should().BeTrue();
            entitiesManagements.Should().ContainKey(storeManagementBennyNike.Guid);


        }
        [Fact]
        public async void NominateNewStoreManager_ReturnFalse_OwnerTriesToNominateNewManagerToAnotherStore()
        {
            // Arrange
            var entitiesOwnerships = new Dictionary<Guid, StoreOwnership>();
            var entitiesManagements = new Dictionary<Guid, StoreManagement>();

            var nikeStore = TestData.CreateStoreObject("nike");
            var adidasStore = TestData.CreateStoreObject("adidas");

            var bennyUser = TestData.CreateUserObject("Benny");
            var oriUser = TestData.CreateUserObject("Ori");

           
            var storeOwnershipBennyAdidas = TestData.CreateStoreOwnershipObject(adidasStore, bennyUser);
           
            entitiesOwnerships[storeOwnershipBennyAdidas.Guid] = storeOwnershipBennyAdidas;

           var us = GetStoreService(null, entitiesOwnerships, null, entitiesManagements, null, null);


            //FAIL:bennyUser is an owner of another store
            var storeManagementsOriNike = TestData.CreateStoreManagementObject(nikeStore, oriUser);
            var newManager = _mapper.Map<StoreManagementDto>(storeManagementsOriNike);
            
            //Act
            var result = await us.NominateNewStoreManager(bennyUser.Guid, newManager);
            
            //Assert
            result.Should().BeFalse();
            entitiesManagements.Should().NotContainKey(storeManagementsOriNike.Guid);
        }
        [Fact]
        public async void NominateNewStoreManager_ReturnFalse_UserThatINotAnOwnerTriesToNominate()
        {
            // Arrange
            var entitiesOwnerships = new Dictionary<Guid, StoreOwnership>();
            var entitiesManagements = new Dictionary<Guid, StoreManagement>();

            var nikeStore = TestData.CreateStoreObject("nike");
            
            var oriUser = TestData.CreateUserObject("Ori");
            var arikUser = TestData.CreateUserObject("Arik");


           var us = GetStoreService(null, entitiesOwnerships, null, entitiesManagements, null, null);


            var storeManagementsOriNike = TestData.CreateStoreManagementObject(nikeStore, oriUser);
            var newManager = _mapper.Map<StoreManagementDto>(storeManagementsOriNike);
            
            //Act
            var result = await us.NominateNewStoreManager(arikUser.Guid, newManager);//FAIL:arikUser is not an owner wanted store
            
            //Assert
            result.Should().BeFalse();
            entitiesManagements.Should().NotContainKey(storeManagementsOriNike.Guid);

        }
        [Fact]
        public async void NominateNewStoreManager_ReturnFalse_OwnerTriesToNominateOtherOwner()
        {
            // Arrange
            var entitiesOwnerships = new Dictionary<Guid, StoreOwnership>();
            var entitiesManagements = new Dictionary<Guid, StoreManagement>();

            var nikeStore = TestData.CreateStoreObject("nike");

            var matanUser = TestData.CreateUserObject("Matan");
            var bennyUser = TestData.CreateUserObject("Benny");
           

            var storeOwnershipMatanNike = TestData.CreateStoreOwnershipObject(nikeStore, matanUser);

            var storeOwnershipBennyAdidas = TestData.CreateStoreOwnershipObject(nikeStore, bennyUser);

            entitiesOwnerships[storeOwnershipMatanNike.Guid] = storeOwnershipMatanNike;
            entitiesOwnerships[storeOwnershipBennyAdidas.Guid] = storeOwnershipBennyAdidas;

           var us = GetStoreService(null, entitiesOwnerships, null, entitiesManagements, null, null);



            var storeManagementBennyNike = TestData.CreateStoreManagementObject(nikeStore, bennyUser);
            var newManager = _mapper.Map<StoreManagementDto>(storeManagementBennyNike);
            
            //Act
            var result = await us.NominateNewStoreManager(matanUser.Guid, newManager);//Fail : both are owners
            
            //Assert
            result.Should().BeFalse();
            entitiesManagements.Should().NotContainKey(storeManagementBennyNike.Guid);

        }
        [Fact]
        public async void NominateNewStoreManager_ReturnFalse_OwnerTriesToNominateOtherStoreManager()
        {
            // Arrange
            var entitiesOwnerships = new Dictionary<Guid, StoreOwnership>();
            var entitiesManagements = new Dictionary<Guid, StoreManagement>();

            var adidasStore = TestData.CreateStoreObject("adidas");

            var bennyUser = TestData.CreateUserObject("Benny");
            var omerUser = TestData.CreateUserObject("Omer");

           

            var storeOwnershipBennyAdidas = TestData.CreateStoreOwnershipObject(adidasStore, bennyUser);
            var storeManagementsOmerAdidas = TestData.CreateStoreManagementObject(adidasStore, omerUser);
            
            entitiesOwnerships[storeOwnershipBennyAdidas.Guid] = storeOwnershipBennyAdidas;
            entitiesManagements[storeManagementsOmerAdidas.Guid] = storeManagementsOmerAdidas;


           var us = GetStoreService(null, entitiesOwnerships, null, entitiesManagements, null, null);


            var newManager = _mapper.Map<StoreManagementDto>(storeManagementsOmerAdidas);
            
            //Act
            var result = await us.NominateNewStoreManager(bennyUser.Guid, newManager);//Fail : already a manager
            
            //Assert
            result.Should().BeFalse();
            
            
        }
        

        [Fact]
        public async void GetSellersInformation_ShouldReturnStoreSellers_WhenStoreExists()
        {
            // Arrange 
            Dictionary<Guid, StoreManagement> entitiesStoreManagements = new Dictionary<Guid, StoreManagement>();
            Dictionary<Guid, StoreOwnership> entitiesStoreOwnerships = new Dictionary<Guid, StoreOwnership>();

            Store storeNike = TestData.GetStoreData("Nike");

            User uBenny = TestData.GetUserData("Benny", "Skidanov", "BennySkidanov");
            User uOmer = TestData.GetUserData("Omer", "Kempner", "OmerKempner");
            User uMatan = TestData.GetUserData("Matan", "Hazan", "MatanHazan");
            User uArye = TestData.GetUserData("Arye", "Shapiro", "BennySkidanov");

            StoreManagement smBenny = TestData.GetStoreManagementData(uBenny, storeNike);
            StoreManagement smOmer = TestData.GetStoreManagementData(uOmer, storeNike);
            StoreOwnership soMatan = TestData.GetStoreOwnershipData(uMatan, storeNike);
            StoreOwnership soArye = TestData.GetStoreOwnershipData(uArye, storeNike);

            entitiesStoreManagements[smBenny.Guid] = smBenny;
            entitiesStoreManagements[smOmer.Guid] = smOmer;
            entitiesStoreOwnerships[soMatan.Guid] = soMatan;
            entitiesStoreOwnerships[soArye.Guid] = soArye;

            var s = GetStoreService(null, entitiesStoreOwnerships, null, entitiesStoreManagements, null, null);

            List<StoreManagementDto> lsm = new List<StoreManagementDto>
            {
                _mapper.Map<StoreManagementDto>(smBenny),
                _mapper.Map<StoreManagementDto>(smOmer)
            };

            List<StoreOwnershipDto> lso = new List<StoreOwnershipDto>
            {
                _mapper.Map<StoreOwnershipDto>(soMatan),
                _mapper.Map<StoreOwnershipDto>(soArye)
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

            var s = GetStoreService(null, entitiesStoreOwnerships, null, entitiesStoreManagements, null, null);

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
            


            Store s1 = TestData.GetStoreData("Adidas");

            User u1 = TestData.GetUserData("Benny", "Skidanov", "BennySkidanov");
            User u2 = TestData.GetUserData("Omer", "Kempner", "OmerKempner");

            StoreManagement sm1 = TestData.GetStoreManagementData(u1, s1);
            StoreManagement sm2 = TestData.GetStoreManagementData(u1, s1);

            StoreManagementPermission smp1 = TestData.GetStoreManagementPermissionData(true, sm1);
            StoreManagementPermission smp2 = TestData.GetStoreManagementPermissionData(false, sm2);

            entitiesStoreManagementPermissions[sm1.Guid] = smp1;
            entitiesStoreManagementPermissions[sm2.Guid] = smp2;


           

            var uow = DalMockFactory.MockStoreUnitOfWork(null, null, null, null, entitiesStoreManagementPermissions, null);


            StoresService s = new(_loggerMock.Object, _mapper, uow.Object);
            

            // Act 
            var res1 = await s.GetPermissions(smp1.Guid);
            var res2 = await s.GetPermissions(smp2.Guid);

            var r1 = _mapper.Map<StoreManagementPermission>(res1);
            var r2 = _mapper.Map<StoreManagementPermission>(res2);

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


            var uow = DalMockFactory.MockStoreUnitOfWork(null, null, null, null, entitiesStoreManagementPermissions, null);


            StoresService s = new(_loggerMock.Object, _mapper, uow.Object);

            // Act 
            var res1 = await s.GetPermissions(new Guid());

            var r1 = _mapper.Map<StoreManagementPermission>(res1);

            // Assert
            r1.Should().BeNull();
        }

        [Fact]
        public async void UpdatePermissions_UpdatePermissionsCorrectly_WhenStoreManagerDtoExist()
        {
            // Arrange
            Dictionary<Guid, StoreManagementPermission> entitiesStoreManagementPermissions =
                new Dictionary<Guid, StoreManagementPermission>();

            Store s1 = TestData.GetStoreData("MaccabiTelAvivFanStore");

            User u1 = TestData.GetUserData("Benny", "Skidanov", "BennySkidanov");

            StoreManagement sm1 = TestData.GetStoreManagementData(u1, s1);

            StoreManagementPermission smp1 = TestData.GetStoreManagementPermissionData(true, sm1);

            entitiesStoreManagementPermissions[smp1.Guid] = smp1;

            var repoPermissions = DalMockFactory.MockRepository(entitiesStoreManagementPermissions);

            var uow = DalMockFactory.MockStoreUnitOfWork(null, null, null, null, entitiesStoreManagementPermissions, null);


            StoresService s = new(_loggerMock.Object, _mapper, uow.Object);


            // Act 
            var replace1 = await s.GetPermissions(smp1.Guid);
            replace1.CanDoSomething = false;

            await s.UpdatePermission(replace1);

            var res1 = await s.GetPermissions(smp1.Guid);

            var r1 = _mapper.Map<StoreManagementPermission>(res1);

            // Assert
            r1.CanDoSomething.Should().BeFalse();
        }

        [Fact]
        public async void UpdatePermissions_UpdatePermissionNotUpdated_WhenSMDoesNotExist()
        {
            // Arrange
            Dictionary<Guid, StoreManagementPermission> entitiesStoreManagementPermissions =
                new Dictionary<Guid, StoreManagementPermission>();
            

            Store s1 = TestData.GetStoreData("MaccabiTelAvivFanStore");

            User u1 = TestData.GetUserData("Benny", "Skidanov", "BennySkidanov");

            StoreManagement sm1 = TestData.GetStoreManagementData(u1, s1);

            StoreManagementPermission smp1 = TestData.GetStoreManagementPermissionData(true, sm1);

            var uow = DalMockFactory.MockStoreUnitOfWork(null, null, null, null, entitiesStoreManagementPermissions, null);

            entitiesStoreManagementPermissions[smp1.Guid] = smp1;
            StoresService s = new(_loggerMock.Object, _mapper, uow.Object);


            // Act 
            var replace1 = await s.GetPermissions(smp1.Guid);
            replace1.CanDoSomething = false;

            await s.UpdatePermission(new StoreManagementPermissionDto());

            var res1 = await s.GetPermissions(smp1.Guid);

            var r1 = _mapper.Map<StoreManagementPermission>(res1);

            // Assert 
            r1.CanDoSomething.Should().BeTrue();
        }

        //[Fact]
        //public async Task DeleteProductAsync_ReturnTrueAndProductIsSafeDeleted_WhenProductExistsAndIsNotSafeDeleted()
        //{
        //    Store s1 = new() { StoreName = "Benny Hadayag" };
        //    Store storeOwnershipBennyAdidas = new() { StoreName = "Nike" };
        //    Store storeManagementsOmerAdidas = new() { StoreName = "Adidas" };
        //    Store s4 = new() { StoreName = "TopShop" };



        //    // Arrange
        //    var productsDict = new Dictionary<Guid, Product>();
        //    var productGuid = Guid.NewGuid();
        //    productsDict[productGuid] = TestData.GetTestProduct(productGuid);

        //    var storeUnitOfWork =
        //        DalMockFactory.MockStoreUnitOfWork(null, null, null, null, null, productsDict);
        //    var sut = new StoresService(loggerMock.Object, mapper, storeUnitOfWork.Object);
            
        //    // Act
        //    var result = await sut.DeleteProductAsync(productGuid);

        //    // Assert
        //    result.Should().BeTrue();
        //    productsDict[productGuid].IsSoftDeleted.Should().BeTrue();
        //}
        
        //[Fact]
        //public async Task DeleteProductAsync_ReturnFalse_WhenProductDoNotNotExist()
        //{
        //    // Arrange
        //    var productsDict = new Dictionary<Guid, Product>();
        //    var storeUnitOfWork =
        //        DalMockFactory.MockStoreUnitOfWork(null, null, null, null, null, productsDict);
        //    var sut = new StoresService(loggerMock.Object, mapper, storeUnitOfWork.Object);

        //    // Act
        //    var result = await sut.DeleteProductAsync(Guid.NewGuid());

        //    // Assert
        //    result.Should().BeFalse();
        //}
        
        //[Fact]
        //public async Task DeleteProductAsync_ReturnFalse_WhenProductExistsAndIsSafeDeleted()
        //{
        //    // Arrange
        //    var productsDict = new Dictionary<Guid, Product>();
        //    var productGuid = Guid.NewGuid();
        //    productsDict[productGuid] = new Product{Guid = productGuid, IsSoftDeleted = true};
        //    var storeUnitOfWork =
        //        DalMockFactory.MockStoreUnitOfWork(null, null, null, null, null, productsDict);
        //    var sut = new StoresService(loggerMock.Object, mapper, storeUnitOfWork.Object);

        //    // Act
        //    var result = await sut.DeleteProductAsync(productGuid);

        //    // Assert
        //    result.Should().BeFalse();
        //    productsDict.Keys.Should().Contain(productGuid);
        //}
        
        //[Fact]
        //public async Task UpdateProductAsync_ReturnsNotSafeDeletedProduct_WhenProductExistsAndNotSafeDeleted()
        //{
        //    // Arrange
        //    var productsDict = new Dictionary<Guid, Product>();
        //    var productToReplaceGuid = Guid.NewGuid();
        //    productsDict[productToReplaceGuid] = TestData.GetTestProduct(productToReplaceGuid);
        //    var storeUnitOfWork =
        //        DalMockFactory.MockStoreUnitOfWork(null, null, null, null, null, productsDict);
        //    var sut = new StoresService(loggerMock.Object, mapper, storeUnitOfWork.Object);

        //    var replacementProductDto =
        //        new ProductDto
        //            {Guid = productToReplaceGuid, Amount = 5, Price = 5, Name = "ChessBoard", Category = "Chess"}; 

        //    // Act
        //    var result = await sut.UpdateProductAsync(replacementProductDto);

        //    // Assert
        //    result.Should().BeTrue();
        //    var resultProduct = productsDict[productToReplaceGuid];
        //    resultProduct.Amount.Should().Be(5);
        //    resultProduct.Price.Should().Be(5);
        //    resultProduct.Category.Should().Be("Chess");
        //    resultProduct.Name.Should().Be("ChessBoard");
        //}
        
        //[Fact]
        //public async Task UpdateProductAsync_ReturnsFalse_WhenProductDoNotExist()
        //{
        //    // Arrange
        //    var productsDict = new Dictionary<Guid, Product>();
        //    var storeUnitOfWork =
        //        DalMockFactory.MockStoreUnitOfWork(null, null, null, null, null, productsDict);
        //    var sut = new StoresService(loggerMock.Object, mapper, storeUnitOfWork.Object);

        //    // Act
        //    var result = await sut.UpdateProductAsync(new ProductDto{Guid = Guid.NewGuid()});

        //    // Assert
        //    result.Should().BeFalse();
        //}
        
        //[Fact]
        //public async Task UpdateProductAsync_ReturnsFalse_WhenProductExistsButIsSafeDeleted()
        //{
        //    // Arrange
        //    var productsDict = new Dictionary<Guid, Product>();
        //    var productGuid = Guid.NewGuid();
        //    productsDict[productGuid] = new Product{Guid = productGuid, IsSoftDeleted = true};
        //    var storeUnitOfWork =
        //        DalMockFactory.MockStoreUnitOfWork(null, null, null, null, null, productsDict);
        //    var sut = new StoresService(loggerMock.Object, mapper, storeUnitOfWork.Object);

        //    // Act
        //    var result = await sut.UpdateProductAsync(new ProductDto{Guid = productGuid});

        //    // Assert
        //    result.Should().BeFalse();
        //}

        //private static User GetUserData(string fName, string lName, string uname)
        //{
        //    var entitiesStores = new Dictionary<Guid, Store>();
        //    var entitiesStorePurchases = new Dictionary<Guid, StorePurchase>();

            
        //    Store s1 = new() { StoreName = "Benny Hadayag" };
        //    Store storeOwnershipBennyAdidas = new() { StoreName = "Nike" };
        //    Store storeManagementsOmerAdidas = new() { StoreName = "Adidas" };
        //    Store s4 = new() { StoreName = "TopShop" };

        //    entitiesStores.Add(s1.Guid, s1);
        //    entitiesStores.Add(storeOwnershipBennyAdidas.Guid, storeOwnershipBennyAdidas);
        //    entitiesStores.Add(storeManagementsOmerAdidas.Guid, storeManagementsOmerAdidas);

        //    var storeUnitOfWork =
        //        DalMockFactory.MockStoreUnitOfWork(entitiesStores, null, entitiesStorePurchases, null, null);

        //    var storesService = new StoresService(_loggerMock.Object, _mapper, storeUnitOfWork.Object);

        //    var res1 = await storesService.GetStoreAsync(s1.Guid);
        //    var expectedRes1 = _mapper.Map<StoreDto>(s1);
        //    res1.Should().BeEquivalentTo(expectedRes1);

        //    var res2 = await storesService.GetStoreAsync(s4.Guid);
        //    res2.Should().BeNull();

        //    var res3 = await storesService.GetStoresAsync();
        //    List<StoreDto> expectedRes3 = new List<StoreDto>();
        //    expectedRes3.Add(_mapper.Map<StoreDto>(s1));
        //    expectedRes3.Add(_mapper.Map<StoreDto>(storeOwnershipBennyAdidas));
        //    expectedRes3.Add(_mapper.Map<StoreDto>(storeManagementsOmerAdidas));
        //    res3.ToList().Should().BeEquivalentTo(expectedRes3);
        //}

        //[Fact]
        //public async void GetStorePurchaseHistoryTest()
        //{
        //    var entitiesStores = new Dictionary<Guid, Store>();
        //    var entitiesStorePurchases = new Dictionary<Guid, StorePurchase>();
            
        //    Store s1 = new() {};
        //    Store storeOwnershipBennyAdidas = new() {};
        //    Store storeManagementsOmerAdidas = new() {};

        //    //nikeStore purchase 
        //        //p1 
        //    var pr1 = new PurchaseProduct();
        //    var pr2 = new PurchaseProduct();
        //    var pr3 = new PurchaseProduct();

        //    var prList1 = new List<PurchaseProduct>();
        //    prList1.Add(pr1);
        //    prList1.Add(pr2);
        //    prList1.Add(pr3);

        //    StorePurchase p1 = new() {PurchaseProducts = prList1 , Store = s1};
               
        //        //p2
        //    var pr4 = new PurchaseProduct();
        //    var pr5 = new PurchaseProduct(); 

        //    var prList2 = new List<PurchaseProduct>();
        //    prList2.Add(pr4);
        //    prList2.Add(pr5);
            

        //    StorePurchase p2 = new() { PurchaseProducts = prList2, Store = s1 };

        //    //adidasStore purchase 
        //    var pr6 = new PurchaseProduct();
        //    var prList3 = new List<PurchaseProduct>();
        //    prList3.Add(pr6);

        //    StorePurchase p3 = new() { PurchaseProducts = prList3, Store = storeOwnershipBennyAdidas };

        //    entitiesStorePurchases.Add(p1.Guid, p1);
        //    entitiesStorePurchases.Add(p2.Guid, p2);
        //    entitiesStorePurchases.Add(p3.Guid, p3);

        //    entitiesStores.Add(s1.Guid,s1);
        //    entitiesStores.Add(storeOwnershipBennyAdidas.Guid, storeOwnershipBennyAdidas);

        //    var storeUnitOfWork =
        //        DalMockFactory.MockStoreUnitOfWork(entitiesStores, null, entitiesStorePurchases, null, null);

        //    var storesService = new StoresService(_loggerMock.Object, _mapper, storeUnitOfWork.Object);

        //    var res1 = await storesService.GetStorePurchaseHistory(s1.Guid);
        //    var expectedRes1 = new List<StorePurchaseDto>();
        //    expectedRes1.Add(_mapper.Map<StorePurchaseDto>(p1));
        //    expectedRes1.Add(_mapper.Map<StorePurchaseDto>(p2));

        //    res1.ToList().Should().BeEquivalentTo(expectedRes1);

        //    var res2 = await storesService.GetStorePurchaseHistory(storeOwnershipBennyAdidas.Guid);
        //    var expectedRes2 = new List<StorePurchaseDto>();
        //    expectedRes2.Add(_mapper.Map<StorePurchaseDto>(p3));

        //    res2.ToList().Should().BeEquivalentTo(expectedRes2);

        //    var res3 = await storesService.GetStorePurchaseHistory(storeManagementsOmerAdidas.Guid);
        //    res3.Should().BeEmpty();
        //}
        
    }
}