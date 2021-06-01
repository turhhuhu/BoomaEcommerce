using System;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BoomaEcommerce.Domain.Discounts;
using BoomaEcommerce.Domain.Discounts.Operators;
using BoomaEcommerce.Domain.Policies;
using BoomaEcommerce.Domain.Policies.Operators;
using BoomaEcommerce.Domain.Policies.PolicyTypes;
using FluentAssertions;
using Xunit;
using Xunit.Abstractions;

namespace BoomaEcommerce.Domain.Tests
{
    public class DiscountTests
    {
        private readonly ITestOutputHelper _testOutputHelper;

        public DiscountTests(ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;
        }

        [Fact]
        public void ApplyDiscount_ShouldApplyDiscountSuccessfullyAndReturnDescriptiveMessage_WhenDiscountIsValid()
        {
            var product = new Product
            {
                Name = "Beer",
                Amount = 20
            };

            var simplePolicy = new AgeRestrictionPolicy(product, 18);

            Discount twentyPercentOff =
                new ProductDiscount(20, DateTime.Now.AddDays(-3), DateTime.Now.AddDays(3), "",
                    simplePolicy, product );

            var buyer = new User { DateOfBirth = DateTime.Now.AddYears(-25), UserName = "matureBenny" };

            List<PurchaseProduct> ppList = new List<PurchaseProduct>();

            var pp = new PurchaseProduct(product, 8, 80); ;
            
            ppList.Add(pp);

            var sp = new StorePurchase {PurchaseProducts = ppList, Buyer = buyer};

            List<StorePurchase> spList = new List<StorePurchase> {sp};


            var purchase = new Purchase {StorePurchases = spList, TotalPrice = 80, Buyer = buyer};

            purchase.TotalPrice.Should().Be(80);

            var result = twentyPercentOff.ApplyDiscount(purchase);

            _testOutputHelper.WriteLine(result);

            purchase.TotalPrice.Should().Be(64);
        }

        [Fact]
        public void ApplyDiscount_ShouldNOTApplyDiscountSuccessfullyAndReturnDescriptiveMessage_WhenDiscountDateIsNOTValid()
        {
            var product = new Product
            {
                Name = "Beer",
                Amount = 20
            };

            var simplePolicy = new AgeRestrictionPolicy(product, 18);

            Discount twentyPercentOff =
                new ProductDiscount(20, DateTime.Now.AddDays(-3), DateTime.Now.AddDays(-1), "",
                    simplePolicy, product);

            var buyer = new User { DateOfBirth = DateTime.Now.AddYears(-25), UserName = "matureBenny" };

            List<PurchaseProduct> ppList = new List<PurchaseProduct>();

            var pp = new PurchaseProduct(product, 8, 80); ;

            ppList.Add(pp);

            var sp = new StorePurchase { PurchaseProducts = ppList, Buyer = buyer };

            List<StorePurchase> spList = new List<StorePurchase> {sp};


            var purchase = new Purchase { StorePurchases = spList, TotalPrice = 80, Buyer = buyer };

            purchase.TotalPrice.Should().Be(80);

            var result = twentyPercentOff.ApplyDiscount(purchase);

            _testOutputHelper.WriteLine(result);

            purchase.TotalPrice.Should().Be(80);
        }

        [Fact]
        public void ApplyDiscount_ShouldNOTApplyDiscountSuccessfullyAndReturnDescriptiveMessage_WhenDiscountPolicyIsNOTFulfilled()
        {
            var product = new Product
            {
                Name = "Beer",
                Amount = 20
            };

            var simplePolicy = new AgeRestrictionPolicy(product, 18);

            Discount twentyPercentOff =
                new ProductDiscount(20, DateTime.Now.AddDays(-3), DateTime.Now.AddDays(1), "",
                    simplePolicy, product);

            var buyer = new User { DateOfBirth = DateTime.Now.AddYears(-16), UserName = "matureBenny" };

            List<PurchaseProduct> ppList = new List<PurchaseProduct>();

            var pp = new PurchaseProduct(product, 8, 80); ;

            ppList.Add(pp);

            var sp = new StorePurchase { PurchaseProducts = ppList, Buyer = buyer };

            List<StorePurchase> spList = new List<StorePurchase> { sp };


            var purchase = new Purchase { StorePurchases = spList, TotalPrice = 80, Buyer = buyer };

            purchase.TotalPrice.Should().Be(80);

            var result = twentyPercentOff.ApplyDiscount(purchase);

            _testOutputHelper.WriteLine(result);

            purchase.TotalPrice.Should().Be(80);
        }


        [Fact]
        public void ApplyDiscount_ShouldApplyBESTDiscountSuccessfullyAndReturnDescriptiveMessage_WhenAllDiscountsAreValid()
        {
            var product = new Product
            {
                Name = "Beer",
                Amount = 20,
                Category = "Alcohol"
            };

            var simplePolicy = new AgeRestrictionPolicy(product, 18);

            Discount twentyPercentOff =
                new ProductDiscount(20, DateTime.Now.AddDays(-3), DateTime.Now.AddDays(3), "",
                    simplePolicy, product);

            Discount tenPercentOff = new CategoryDiscount(10, DateTime.Now.AddDays(-3), DateTime.Now.AddDays(3), "",
                simplePolicy, "Alcohol");

            var buyer = new User { DateOfBirth = DateTime.Now.AddYears(-25), UserName = "matureBenny" };

            List<PurchaseProduct> ppList = new List<PurchaseProduct>();

            var compositeDiscount = new CompositeDiscount(new MaxDiscountOperator());

            compositeDiscount.AddToDiscountList(twentyPercentOff);

            compositeDiscount.AddToDiscountList(tenPercentOff);

            var pp = new PurchaseProduct(product, 10, 100); ;

            ppList.Add(pp);

            var sp = new StorePurchase { PurchaseProducts = ppList, Buyer = buyer };

            List<StorePurchase> spList = new List<StorePurchase>();

            spList.Add(sp);

            var purchase = new Purchase { StorePurchases = spList, TotalPrice = 100, Buyer = buyer };

            purchase.TotalPrice.Should().Be(100);

            var result = compositeDiscount.ApplyDiscount(purchase);

            _testOutputHelper.WriteLine(result);

            purchase.TotalPrice.Should().Be(80);
        }

        [Fact]
        public void ApplyDiscount_ShouldApplyALLDiscountsSuccessfullyAndReturnDescriptiveMessage_WhenAllDiscountsAreValid()
        {
            Guid storeGuid = Guid.NewGuid();
            Store s = new Store {Guid = storeGuid, StoreName = "Sarah Mashkaot"};
            var product = new Product
            {
                Name = "Beer",
                Amount = 20,
                Category = "Alcohol",
                Store = s
            };

            var simplePolicy = new AgeRestrictionPolicy(product, 18);

            Discount twentyPercentOff =
                new ProductDiscount(20, DateTime.Now.AddDays(-3), DateTime.Now.AddDays(3), "",
                    simplePolicy, product);

            Discount tenPercentOff = new CategoryDiscount(10, DateTime.Now.AddDays(-3), DateTime.Now.AddDays(3), "",
                simplePolicy, "Alcohol");
            
            Discount fivePercentOff = new BasketDiscount(5, DateTime.Now.AddDays(-3), DateTime.Now.AddDays(3), "",
                simplePolicy, s);

            var buyer = new User { DateOfBirth = DateTime.Now.AddYears(-25), UserName = "matureBenny" };

            List<PurchaseProduct> ppList = new List<PurchaseProduct>();

            var compositeDiscount = new CompositeDiscount(new SumDiscountOperator());

            compositeDiscount.AddToDiscountList(twentyPercentOff);

            compositeDiscount.AddToDiscountList(tenPercentOff);

            compositeDiscount.AddToDiscountList(fivePercentOff);

            var pp = new PurchaseProduct(product, 10, 100); ;

            ppList.Add(pp);

            var sp = new StorePurchase { PurchaseProducts = ppList, Buyer = buyer, Store = s};

            List<StorePurchase> spList = new List<StorePurchase>();

            spList.Add(sp);

            var purchase = new Purchase { StorePurchases = spList, TotalPrice = 100, Buyer = buyer};

            purchase.TotalPrice.Should().Be(100);

            var result = compositeDiscount.ApplyDiscount(purchase);

            _testOutputHelper.WriteLine(result);

            purchase.TotalPrice.Should().Be((decimal)68.4);
        }
    }


}
