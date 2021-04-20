using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using BoomaEcommerce.Domain;
using BoomaEcommerce.Services.DTO;
using BoomaEcommerce.Services.Stores;
using BoomaEcommerce.Tests.CoreLib;
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
        public async Task NominateNewStoreOwner_ReturnTrue_NewOwnerDoesNotHaveOtherResponsibilities()
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
        public async Task NominateNewStoreOwner_ReturnFalse_OwnerOfAStoreTriesToNominateNewOwnerToAnotherStore()
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
        public async Task NominateNewStoreOwner_ReturnFalse_UserThatIsNotAnOwnerTriesToNominate()
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
        public async Task NominateNewStoreOwner_ReturnFalse_OwnerTriesToNominateOtherOwner()
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
        public async Task NominateNewStoreOwner_ReturnFalse_OwnerTriesToNominateOtherStoreManager()
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
        public async Task NominateNewStoreManager_ReturnTrue_NewManagerDoesNotHaveOtherResponsibilities()
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
            newManager.Guid = Guid.Empty;
            //Act
            var result = await us.NominateNewStoreManager(matanUser.Guid, newManager);
            
            //Assert
            result.Should().BeTrue();
        }
        
        [Fact]
        public async Task NominateNewStoreManager_ReturnFalse_OwnerTriesToNominateNewManagerToAnotherStore()
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
            newManager.Guid = Guid.Empty;
            
            //Act
            var result = await us.NominateNewStoreManager(bennyUser.Guid, newManager);
            
            //Assert
            result.Should().BeFalse();
            entitiesManagements.Should().NotContainKey(storeManagementsOriNike.Guid);
        }
        [Fact]
        public async Task NominateNewStoreManager_ReturnFalse_UserThatINotAnOwnerTriesToNominate()
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
            newManager.Guid = Guid.Empty;
            //Act
            var result = await us.NominateNewStoreManager(arikUser.Guid, newManager);//FAIL:arikUser is not an owner wanted store
            
            //Assert
            result.Should().BeFalse();
            entitiesManagements.Should().NotContainKey(storeManagementsOriNike.Guid);

        }
        [Fact]
        public async Task NominateNewStoreManager_ReturnFalse_OwnerTriesToNominateOtherOwner()
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
            newManager.Guid = Guid.Empty;
            //Act
            var result = await us.NominateNewStoreManager(matanUser.Guid, newManager);//Fail : both are owners
            
            //Assert
            result.Should().BeFalse();
            entitiesManagements.Should().NotContainKey(storeManagementBennyNike.Guid);

        }
        [Fact]
        public async Task NominateNewStoreManager_ReturnFalse_OwnerTriesToNominateOtherStoreManager()
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
            newManager.Guid = Guid.Empty;
            //Act
            var result = await us.NominateNewStoreManager(bennyUser.Guid, newManager);//Fail : already a manager
            
            //Assert
            result.Should().BeFalse();
            
            
        }
        

        [Fact]
        public async Task GetSellersInformation_ShouldReturnStoreSellers_WhenStoreExists()
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
        public async Task GetSellersInformation_ShouldReturnEmptyObject_WhenStoreDoesNotExist()
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
        public async Task GetPermissions_ShouldReturnCorrectPermissions_WhenSMExists()
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
            r1.CanAddProduct.Should().BeTrue();
            r2.CanAddProduct.Should().BeFalse();
        }

        [Fact]
        public async Task GetPermissions_ShouldReturnNull_WhenSMDoesNotExist()
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
        public async Task UpdatePermissions_UpdatePermissionsCorrectly_WhenStoreManagerDtoExist()
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
            replace1.CanAddProduct = false;

            await s.UpdatePermission(replace1);

            var res1 = await s.GetPermissions(smp1.Guid);

            var r1 = _mapper.Map<StoreManagementPermission>(res1);

            // Assert
            r1.CanAddProduct.Should().BeFalse();
        }

        [Fact]
        public async Task UpdatePermissions_UpdatePermissionNotUpdated_WhenSMDoesNotExist()
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
            replace1.CanAddProduct = false;

            await s.UpdatePermission(new StoreManagementPermissionDto());

            var res1 = await s.GetPermissions(smp1.Guid);

            var r1 = _mapper.Map<StoreManagementPermission>(res1);

            // Assert 
            r1.CanAddProduct.Should().BeTrue();
        }

        [Fact]
        public async Task DeleteProductAsync_ReturnTrueAndProductIsSafeDeleted_WhenProductExistsAndIsNotSafeDeleted()
        {
            //Arrange
            var productsDict = new Dictionary<Guid, Product>();
            var productGuid = Guid.NewGuid();
            productsDict[productGuid] = TestData.GetTestProduct(productGuid);

            var sut = GetStoreService(null, null, null, null, null, productsDict);
            
            //Act
            var result = await sut.DeleteProductAsync(productGuid);

            //Assert
            result.Should().BeTrue();
            productsDict[productGuid].IsSoftDeleted.Should().BeTrue();
        }
        
        [Fact]
        public async Task DeleteProductAsync_ReturnFalse_WhenProductDoNotNotExist()
        {
            //Arrange
            var productsDict = new Dictionary<Guid, Product>();
            var sut = GetStoreService(null, null, null, null, null, productsDict);

            //Act
            var result = await sut.DeleteProductAsync(Guid.NewGuid());

            //Assert
            result.Should().BeFalse();
        }
        
        [Fact]
        public async Task DeleteProductAsync_ReturnFalse_WhenProductExistsAndIsSafeDeleted()
        {
            //Arrange
            var productsDict = new Dictionary<Guid, Product>();
            var productGuid = Guid.NewGuid();
            productsDict[productGuid] = new Product{Guid = productGuid, IsSoftDeleted = true};
            var sut = GetStoreService(null, null, null, null, null, productsDict);

            //Act
            var result = await sut.DeleteProductAsync(productGuid);

            //Assert
            result.Should().BeFalse();
            productsDict.Keys.Should().Contain(productGuid);
        }

        [Fact]
        public async Task UpdateProductAsync_ReturnsNotSafeDeletedProduct_WhenProductExistsAndNotSafeDeleted()
        {
            //Arrange
            var productsDict = new Dictionary<Guid, Product>();
            var storesDict = new Dictionary<Guid, Store>();
            var storeGuid = Guid.NewGuid();
            storesDict[storeGuid] = new Store {Guid = storeGuid};

            var productToReplaceGuid = Guid.NewGuid();
            productsDict[productToReplaceGuid] = TestData.GetTestProduct(productToReplaceGuid, storeGuid);
            var sut = GetStoreService(storesDict, null, null, null, null, productsDict);

            var replacementProductDto =
                new ProductDto
                {
                    Guid = productToReplaceGuid,
                    Amount = 5,
                    Price = 5,
                    Name = "ChessBoard",
                    Category = "Chess",
                    Store = new StoreDto{Guid = storeGuid}
                }; 

            //Act
            var result = await sut.UpdateProductAsync(replacementProductDto);

            //Assert
            result.Should().BeTrue();
            var resultProduct = productsDict[productToReplaceGuid];
            resultProduct.Amount.Should().Be(5);
            resultProduct.Price.Should().Be(5);
            resultProduct.Category.Should().Be("Chess");
            resultProduct.Name.Should().Be("ChessBoard");
        }
        
        [Fact]
        public async Task UpdateProductAsync_ReturnsFalse_WhenProductDoNotExist()
        {
            //Arrange
            var productsDict = new Dictionary<Guid, Product>();
            var sut = GetStoreService(null, null, null, null, null, productsDict);

            //Act
            var result = await sut.UpdateProductAsync(new ProductDto
            {
                Guid = Guid.NewGuid(),
                Store = new StoreDto{Guid = Guid.NewGuid()}
            });

            //Assert
            result.Should().BeFalse();
        }
        
        [Fact]
        public async Task UpdateProductAsync_ReturnsFalse_WhenProductExistsButIsSafeDeleted()
        {
            //Arrange
            var productsDict = new Dictionary<Guid, Product>();
            var productGuid = Guid.NewGuid();
            productsDict[productGuid] = new Product{Guid = productGuid, IsSoftDeleted = true};
            var sut = GetStoreService(null, null, null, null, null, productsDict);

            //Act
            var result = await sut.UpdateProductAsync(new ProductDto
            {
                Guid = productGuid,
                Store = new StoreDto{Guid = Guid.NewGuid()}
            });

            //Assert
            result.Should().BeFalse();
        }


        [Fact]
        public async Task GetStores_ShouldReturnStores_WhenStoresExists()
        {
            //Arrange
            var entitiesStores = new Dictionary<Guid, Store>();
            var entitiesStorePurchases = new Dictionary<Guid, StorePurchase>();

            Guid storeBennyGuid = Guid.NewGuid();
            Store storeBenny = TestData.CreateStoreObject("Benny Hadayag", storeBennyGuid);
            Guid storeNikeGuid = Guid.NewGuid();
            Store storeNike = TestData.CreateStoreObject("Nike", storeNikeGuid);
            Guid storeAdidasGuid = Guid.NewGuid();
            Store storeAdidas = TestData.CreateStoreObject("Adidas", storeAdidasGuid);

            entitiesStores.Add(storeBenny.Guid, storeBenny);
            entitiesStores.Add(storeNike.Guid, storeNike);
            entitiesStores.Add(storeAdidas.Guid, storeAdidas);

            var storesService = GetStoreService(entitiesStores, null, entitiesStorePurchases, null, null, null);

            //Act
            var res = await storesService.GetStoresAsync();
            
            //Assert
            List<StoreDto> expectedRes = new List<StoreDto>();
            expectedRes.Add(_mapper.Map<StoreDto>(storeBenny));
            expectedRes.Add(_mapper.Map<StoreDto>(storeNike));
            expectedRes.Add(_mapper.Map<StoreDto>(storeAdidas));
            res.ToList().Should().BeEquivalentTo(expectedRes);

        }

        [Fact]
        public async Task GetStoreData_ShouldReturnNull_WhenStoreDoesNotExists()
        {
            //Arrange
            var entitiesStores = new Dictionary<Guid, Store>();
            var entitiesStorePurchases = new Dictionary<Guid, StorePurchase>();
            
            Guid storeTopShopGuid = Guid.NewGuid();
            Store storeTopShop = TestData.CreateStoreObject("TopShop", storeTopShopGuid);

            var storesService = GetStoreService(entitiesStores, null, entitiesStorePurchases, null, null, null);

            //Act
            var res = await storesService.GetStoreAsync(storeTopShop.Guid);
            
            //Assert
            res.Should().BeNull();
        }

        [Fact]
        public async Task GetStoreData_ShouldReturnStore_WhenStoreExists()
        {
            //Arrange
            var entitiesStores = new Dictionary<Guid, Store>();
            var entitiesStorePurchases = new Dictionary<Guid, StorePurchase>();

            Guid storeBennyGuid = Guid.NewGuid();
            Store storeBenny = TestData.CreateStoreObject("Benny Hadayag", storeBennyGuid);

            entitiesStores.Add(storeBenny.Guid, storeBenny);

            var storesService = GetStoreService(entitiesStores, null, entitiesStorePurchases, null, null, null);
            //Act
            var res = await storesService.GetStoreAsync(storeBenny.Guid);
            
            //Assert
            var expectedRes = _mapper.Map<StoreDto>(storeBenny);
            res.Should().BeEquivalentTo(expectedRes);
        }


        [Fact]
        public async Task GetStorePurchaseHistory_ShouldReturnPurchaseList_WhenPurchasesExists()
        {
            //Arrange
            var entitiesStores = new Dictionary<Guid, Store>();
            var entitiesStorePurchases = new Dictionary<Guid, StorePurchase>();

            Store store_Adidas = TestData.CreateStoreObject("Adidas");
            Store store_Nike = TestData.CreateStoreObject("Nike");

            var banana_pr = TestData.CreatePurchaseProductObject(TestData.CreateProductObject("banana"));
            var coffee_pr = TestData.CreatePurchaseProductObject(TestData.CreateProductObject("coffee"));
            var apple_pr = TestData.CreatePurchaseProductObject(TestData.CreateProductObject("apple"));

            var prList1 = new List<PurchaseProduct>();
            prList1.Add(banana_pr);
            prList1.Add(coffee_pr);
            prList1.Add(apple_pr);

            Guid p1_guid = Guid.NewGuid();
            StorePurchase p1 = TestData.CreateStorePurchaseObject(store_Adidas, prList1, p1_guid);


            var mango_pr = new PurchaseProduct();
            var melon_pr = new PurchaseProduct();

            var prList2 = new List<PurchaseProduct>();
            prList2.Add(mango_pr);
            prList2.Add(melon_pr);

            Guid p2_guid = Guid.NewGuid();
            StorePurchase p2 = TestData.CreateStorePurchaseObject(store_Adidas, prList2, p2_guid);

            entitiesStorePurchases.Add(p1.Guid, p1);
            entitiesStorePurchases.Add(p2.Guid, p2);
 
            entitiesStores.Add(store_Adidas.Guid, store_Adidas);
            entitiesStores.Add(store_Nike.Guid, store_Nike);

            var storesService = GetStoreService(entitiesStores, null, entitiesStorePurchases, null, null, null);

            //Act
            var res = await storesService.GetStorePurchaseHistory(store_Adidas.Guid);

            //Assert
            var expectedRes = new List<StorePurchaseDto>();
            expectedRes.Add(_mapper.Map<StorePurchaseDto>(p1));
            expectedRes.Add(_mapper.Map<StorePurchaseDto>(p2));

            res.ToList().Should().BeEquivalentTo(expectedRes);
        }

        [Fact]
        public async Task GetStorePurchaseHistory_ShouldReturnEmpty_WhenPurchaseHistoryEmpty()
        {
            //Arrange
            var entitiesStores = new Dictionary<Guid, Store>();
            var entitiesStorePurchases = new Dictionary<Guid, StorePurchase>();

            Store store_Ikea = TestData.CreateStoreObject("Ikea");
            var storesService = GetStoreService(entitiesStores, null, entitiesStorePurchases, null, null, null);

            //Act
            var res3 = await storesService.GetStorePurchaseHistory(store_Ikea.Guid);
            
            //Assert
            res3.Should().BeEmpty();
        }
        
    }
}