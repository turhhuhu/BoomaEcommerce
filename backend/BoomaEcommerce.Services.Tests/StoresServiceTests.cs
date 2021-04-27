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
        public async Task NominateNewStoreOwnerAsync_ReturnTrue_WhenNewOwnerDoesNotHaveOtherResponsibilities()
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
            
            var storeOwnershipBennyNike = TestData.CreateStoreOwnershipObject(nikeStore, bennyUser);
            var newOwner = _mapper.Map<StoreOwnershipDto>(storeOwnershipBennyNike);
            newOwner.Guid = Guid.Empty;
            //Act
            var result = await us.NominateNewStoreOwnerAsync(storeOwnershipMatanNike.Guid, newOwner);
            
            //Assert
            result.Should().BeTrue();
            entitiesOwnerships.Count.Should().Be(2);
        }
        
        [Fact]
        public async Task NominateNewStoreOwnerAsync_ReturnFalse_WhenOwnerOfAStoreTriesToNominateNewOwnerToAnotherStore()
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

            
            var storeOwnershipOriNike = TestData.CreateStoreOwnershipObject(nikeStore, oriUser);
            var newOwner = _mapper.Map<StoreOwnershipDto>(storeOwnershipOriNike);
            newOwner.Guid = Guid.Empty;
            //Act
            var result = await us.NominateNewStoreOwnerAsync(bennyUser.Guid, newOwner);
            
            //Assert
            result.Should().BeFalse();
            entitiesOwnerships.Count.Should().Be(1);

        }
        [Fact]
        public async Task NominateNewStoreOwnerAsync_ReturnFalse_WhenUserThatIsNotAnOwnerTriesToNominate()
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
            newOwner.Guid = Guid.Empty;
            //Act
            var result = await us.NominateNewStoreOwnerAsync(arikUser.Guid, newOwner);
            
            //Assert
            result.Should().BeFalse();
            entitiesOwnerships.Should().BeEmpty();



        }
        [Fact]
        public async Task NominateNewStoreOwnerAsync_ReturnFalse_WhenOwnerTriesToNominateOtherOwner()
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
            newOwner.Guid = Guid.Empty;
            //Act
            var result = await us.NominateNewStoreOwnerAsync(matanUser.Guid, newOwner);
            
            //Assert
            result.Should().BeFalse();
            entitiesOwnerships.Count.Should().Be(2);
        }
        
        [Fact]
        public async Task NominateNewStoreOwnerAsync_ReturnFalse_WhenOwnerTriesToNominateOtherStoreManager()
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
            newOwner.Guid = Guid.Empty;
            
            //Act
            var result = await us.NominateNewStoreOwnerAsync(bennyUser.Guid, newOwner);
            
            //Assert
            result.Should().BeFalse();
            entitiesOwnerships.Count.Should().Be(1);

        }


        [Fact]
        public async Task NominateNewStoreManagerAsync_ReturnTrue_WhenNewManagerDoesNotHaveOtherResponsibilities()
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
            var result = await us.NominateNewStoreManagerAsync(storeOwnershipMatanNike.Guid, newManager);
            
            //Assert
            result.Should().BeTrue();
            entitiesManagements.Count.Should().Be(1);
        }
        
        [Fact]
        public async Task NominateNewStoreManagerAsync_ReturnFalse_WhenOwnerTriesToNominateNewManagerToAnotherStore()
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


            //FAIL:bennyUser is an owner of another UserStore
            var storeManagementsOriNike = TestData.CreateStoreManagementObject(nikeStore, oriUser);
            var newManager = _mapper.Map<StoreManagementDto>(storeManagementsOriNike);
            newManager.Guid = Guid.Empty;
            
            //Act
            var result = await us.NominateNewStoreManagerAsync(bennyUser.Guid, newManager);
            
            //Assert
            result.Should().BeFalse();
            entitiesManagements.Should().BeEmpty();
        }
        [Fact]
        public async Task NominateNewStoreManagerAsync_ReturnFalse_WhenUserThatINotAnOwnerTriesToNominate()
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
            var result = await us.NominateNewStoreManagerAsync(arikUser.Guid, newManager);//FAIL:arikUser is not an owner wanted UserStore
            
            //Assert
            result.Should().BeFalse();
            entitiesManagements.Should().BeEmpty();
        }
        [Fact]
        public async Task NominateNewStoreManagerAsync_ReturnFalse_WhenOwnerTriesToNominateOtherOwner()
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
            var result = await us.NominateNewStoreManagerAsync(matanUser.Guid, newManager);//Fail : both are owners
            
            //Assert
            result.Should().BeFalse();
            entitiesManagements.Should().BeEmpty();

        }
        [Fact]
        public async Task NominateNewStoreManagerAsync_ReturnFalse_WhenOwnerTriesToNominateOtherStoreManager()
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
            var result = await us.NominateNewStoreManagerAsync(bennyUser.Guid, newManager);
            
            //Assert
            result.Should().BeFalse();
            entitiesManagements.Count.Should().Be(1);
        }
        

        [Fact]
        public async Task GetSellersInformationAsync_ShouldReturnStoreSellers_WhenStoreExists()
        {
            // Arrange 
            var entitiesStoreManagements = new Dictionary<Guid, StoreManagement>();
            var entitiesStoreOwnerships = new Dictionary<Guid, StoreOwnership>();

            var storeNike = TestData.GetStoreData("Nike");

            var uBenny = TestData.GetUserData("Benny", "Skidanov", "BennySkidanov");
            var uOmer = TestData.GetUserData("Omer", "Kempner", "OmerKempner");
            var uMatan = TestData.GetUserData("Matan", "Hazan", "MatanHazan");
            var uArye = TestData.GetUserData("Arye", "Shapiro", "BennySkidanov");

            var smBenny = TestData.GetStoreManagementData(uBenny, storeNike);

            var smOmer = TestData.GetStoreManagementData(uOmer, storeNike);
            var soMatan = TestData.GetStoreOwnershipData(uMatan, storeNike);
            var soArye = TestData.GetStoreOwnershipData(uArye, storeNike);

            entitiesStoreManagements[smBenny.Guid] = smBenny;
            entitiesStoreManagements[smOmer.Guid] = smOmer;
            entitiesStoreOwnerships[soMatan.Guid] = soMatan;
            entitiesStoreOwnerships[soArye.Guid] = soArye;

            var s = GetStoreService(null, entitiesStoreOwnerships, null,
                entitiesStoreManagements, null, null);

            var lsm = new List<StoreManagementDto>
            {
                _mapper.Map<StoreManagementDto>(smBenny),
                _mapper.Map<StoreManagementDto>(smOmer)
            };

            var lso = new List<StoreOwnershipDto>
            {
                _mapper.Map<StoreOwnershipDto>(soMatan),
                _mapper.Map<StoreOwnershipDto>(soArye)
            };

            // Act 
            var response = await s.GetAllSellersInformationAsync(storeNike.Guid);

            // Assert
            var expectedResponse = new StoreSellersResponse
            {
                StoreManagers = lsm,
                StoreOwners = lso
            };
            response.Should().BeEquivalentTo(expectedResponse);
        }

        [Fact]
        public async Task GetSellersInformationAsync_ShouldReturnEmptyObject_WhenStoreDoesNotExist()
        {
            // Arrange 
            var entitiesStoreManagements = new Dictionary<Guid, StoreManagement>();
            var entitiesStoreOwnerships = new Dictionary<Guid, StoreOwnership>();

            var s = GetStoreService(null, entitiesStoreOwnerships, null,
                entitiesStoreManagements, null, null);

            // Act 
            var response = await s.GetAllSellersInformationAsync(Guid.NewGuid());

            // Assert 
            response.StoreOwners.Should().BeEmpty();
            response.StoreManagers.Should().BeEmpty();
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
                    Rating = 3,
                    StoreGuid = storeGuid
                }; 

            //Act
            var result = await sut.UpdateProductAsync(replacementProductDto);

            //Assert
            result.Should().BeTrue();
            var resultProduct = productsDict[productToReplaceGuid];
            resultProduct.Should().BeEquivalentTo(replacementProductDto, 
                opt => opt
                    .Excluding(p => p.Guid)
                    .Excluding(p => p.StoreGuid));
        }
        
        [Fact]
        public async Task UpdateProductAsync_ReturnsFalse_WhenProductDoNotExist()
        {
            //Arrange
            var productsDict = new Dictionary<Guid, Product>();
            var sut = GetStoreService(null, null, null, null, 
                null, productsDict);
            var productDto = new ProductDto
            {
                Guid = Guid.NewGuid(),
                StoreGuid = Guid.NewGuid()
            };
            
            //Act
            var result = await sut.UpdateProductAsync(productDto);

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
            var sut = GetStoreService(null, null, null,
                null, null, productsDict);
            var productDto = new ProductDto
            {
                Guid = Guid.NewGuid(),
                StoreGuid = Guid.NewGuid()
            };

            //Act
            var result = await sut.UpdateProductAsync(productDto);

            //Assert
            result.Should().BeFalse();
        }


        [Fact]
        public async Task GetStoresAsync_ShouldReturnStores_WhenStoresExists()
        {
            //Arrange
            var entitiesStores = new Dictionary<Guid, Store>();
            var entitiesStorePurchases = new Dictionary<Guid, StorePurchase>();

            var storeBennyGuid = Guid.NewGuid();
            var storeBenny = TestData.CreateStoreObject("Benny Hadayag", storeBennyGuid);
            var storeNikeGuid = Guid.NewGuid();
            var storeNike = TestData.CreateStoreObject("Nike", storeNikeGuid);
            var storeAdidasGuid = Guid.NewGuid();
            var storeAdidas = TestData.CreateStoreObject("Adidas", storeAdidasGuid);

            entitiesStores.Add(storeBenny.Guid, storeBenny);
            entitiesStores.Add(storeNike.Guid, storeNike);
            entitiesStores.Add(storeAdidas.Guid, storeAdidas);

            var storesService = GetStoreService(entitiesStores, null, entitiesStorePurchases,
                null, null, null);

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
        public async Task GetStoreDataAsync_ShouldReturnNull_WhenStoreDoesNotExists()
        {
            //Arrange
            var entitiesStores = new Dictionary<Guid, Store>();
            var entitiesStorePurchases = new Dictionary<Guid, StorePurchase>();
            
            var storeTopShopGuid = Guid.NewGuid();
            var storeTopShop = TestData.CreateStoreObject("TopShop", storeTopShopGuid);

            var storesService = GetStoreService(entitiesStores, null, entitiesStorePurchases,
                null, null, null);

            //Act
            var res = await storesService.GetStoreAsync(storeTopShop.Guid);
            
            //Assert
            res.Should().BeNull();
        }

        [Fact]
        public async Task GetStoreDataAsync_ShouldReturnStore_WhenStoreExists()
        {
            //Arrange
            var entitiesStores = new Dictionary<Guid, Store>();
            var entitiesStorePurchases = new Dictionary<Guid, StorePurchase>();

            var storeBennyGuid = Guid.NewGuid();
            var storeBenny = TestData.CreateStoreObject("Benny Hadayag", storeBennyGuid);

            entitiesStores.Add(storeBenny.Guid, storeBenny);

            var storesService = GetStoreService(entitiesStores, null, entitiesStorePurchases,
                null, null, null);
            //Act
            var res = await storesService.GetStoreAsync(storeBenny.Guid);
            
            //Assert
            var expectedRes = _mapper.Map<StoreDto>(storeBenny);
            res.Should().BeEquivalentTo(expectedRes);
        }


        [Fact]
        public async Task GetStorePurchaseHistoryAsync_ShouldReturnPurchaseList_WhenPurchasesExists()
        {
            //Arrange
            var entitiesStores = new Dictionary<Guid, Store>();
            var entitiesStorePurchases = new Dictionary<Guid, StorePurchase>();

            var store_Adidas = TestData.CreateStoreObject("Adidas");
            var store_Nike = TestData.CreateStoreObject("Nike");

            var banana_pr = TestData.CreatePurchaseProductObject(TestData.CreateProductObject("banana"));
            var coffee_pr = TestData.CreatePurchaseProductObject(TestData.CreateProductObject("coffee"));
            var apple_pr = TestData.CreatePurchaseProductObject(TestData.CreateProductObject("apple"));

            var prList1 = new List<PurchaseProduct>();
            prList1.Add(banana_pr);
            prList1.Add(coffee_pr);
            prList1.Add(apple_pr);

            var p1_guid = Guid.NewGuid();
            var p1 = TestData.CreateStorePurchaseObject(store_Adidas, prList1, p1_guid);
            
            var mango_pr = new PurchaseProduct();
            var melon_pr = new PurchaseProduct();
            
            var prList2 = new List<PurchaseProduct>();
            prList2.Add(mango_pr);
            prList2.Add(melon_pr);

            var p2_guid = Guid.NewGuid();
            var p2 = TestData.CreateStorePurchaseObject(store_Adidas, prList2, p2_guid);

            entitiesStorePurchases.Add(p1.Guid, p1);
            entitiesStorePurchases.Add(p2.Guid, p2);
 
            entitiesStores.Add(store_Adidas.Guid, store_Adidas);
            entitiesStores.Add(store_Nike.Guid, store_Nike);

            var storesService = GetStoreService(entitiesStores, null, entitiesStorePurchases, 
                null, null, null);

            //Act
            var res = await storesService.GetStorePurchaseHistoryAsync(store_Adidas.Guid);

            //Assert
            var expectedRes = new List<StorePurchaseDto>();
            expectedRes.Add(_mapper.Map<StorePurchaseDto>(p1));
            expectedRes.Add(_mapper.Map<StorePurchaseDto>(p2));

            res.ToList().Should().BeEquivalentTo(expectedRes);
        }

        [Fact]
        public async Task GetStorePurchaseHistoryAsync_ShouldReturnEmpty_WhenPurchaseHistoryEmpty()
        {
            //Arrange
            var entitiesStores = new Dictionary<Guid, Store>();
            var entitiesStorePurchases = new Dictionary<Guid, StorePurchase>();

            var store_Ikea = TestData.CreateStoreObject("Ikea");
            var storesService = GetStoreService(entitiesStores, null, entitiesStorePurchases,
                null, null, null);

            //Act
            var res = await storesService.GetStorePurchaseHistoryAsync(store_Ikea.Guid);
            
            //Assert
            res.Should().BeEmpty();
        }
        
    }
}