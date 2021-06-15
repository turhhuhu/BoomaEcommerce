using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BoomaEcommerce.Data;
using BoomaEcommerce.Data.EfCore;
using BoomaEcommerce.Data.InMemory;
using BoomaEcommerce.Domain;
using BoomaEcommerce.Domain.ProductOffer;
using Microsoft.AspNetCore.Identity;

namespace BoomaEcommerce.Tests.CoreLib
{
    public class PurchaseUnitOfWorkStub : PurchaseUnitOfWork
    {
        public PurchaseUnitOfWorkStub(ApplicationDbContext dbContext, IRepository<Purchase> purchaseRepo, UserManager<User> userRepo, IRepository<Product> productRepo, IRepository<ShoppingCart> shoppingCartRepo, IRepository<StoreOwnership> storeOwnerShipRepo, IRepository<Store> storeRepo, IRepository<ProductOffer> offersRepo, IRepository<ShoppingBasket> shoppingBasketRepo) 
            : base(dbContext, purchaseRepo, userRepo, productRepo, shoppingCartRepo, storeOwnerShipRepo, storeRepo, offersRepo, shoppingBasketRepo)
        {
        }

        public override async Task<ITransactionContext> BeginTransaction()
        {
            return new InMemoryPurchaseUnitOfWork.StubTransactionContext();
        }
    }
}
