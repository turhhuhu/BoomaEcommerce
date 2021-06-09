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

            var pp = new PurchaseProduct(product, 8, 80, 80); ;
            
            ppList.Add(pp);

            var sp = new StorePurchase {PurchaseProducts = ppList, Buyer = buyer, TotalPrice = 80, DiscountedPrice = 80 };

            List<StorePurchase> spList = new List<StorePurchase> {sp};


            var purchase = new Purchase {StorePurchases = spList, TotalPrice = 80, Buyer = buyer};

            sp.TotalPrice.Should().Be(80);

            var result = twentyPercentOff.ApplyDiscount(sp);

            _testOutputHelper.WriteLine(result);

            sp.DiscountedPrice.Should().Be(64);
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

            var sp = new StorePurchase { PurchaseProducts = ppList, Buyer = buyer, TotalPrice =  80, DiscountedPrice = 80};

            List<StorePurchase> spList = new List<StorePurchase> {sp};


            var purchase = new Purchase { StorePurchases = spList, TotalPrice = 80, Buyer = buyer };

            sp.TotalPrice.Should().Be(80);

            var result = twentyPercentOff.ApplyDiscount(sp);

            _testOutputHelper.WriteLine(result);

            sp.DiscountedPrice.Should().Be(80);
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

            var sp = new StorePurchase { PurchaseProducts = ppList, Buyer = buyer, TotalPrice = 80, DiscountedPrice = 80};

            List<StorePurchase> spList = new List<StorePurchase> { sp };


            var purchase = new Purchase { StorePurchases = spList, TotalPrice = 80, Buyer = buyer };

            sp.TotalPrice.Should().Be(80);

            var result = twentyPercentOff.ApplyDiscount(sp);

            _testOutputHelper.WriteLine(result);

            sp.TotalPrice.Should().Be(80);
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

            var pp = new PurchaseProduct(product, 10, 100, 100); ;

            ppList.Add(pp);

            var sp = new StorePurchase { PurchaseProducts = ppList, Buyer = buyer, TotalPrice = 100, DiscountedPrice = 100};

            List<StorePurchase> spList = new List<StorePurchase>();

            spList.Add(sp);

            var purchase = new Purchase { StorePurchases = spList, TotalPrice = 100, Buyer = buyer };

            sp.TotalPrice.Should().Be(100);

            var result = compositeDiscount.ApplyDiscount(sp);

            _testOutputHelper.WriteLine(result);

            sp.DiscountedPrice.Should().Be(80);
        }

        [Fact]
        public void ApplyDiscount_ShouldApplyALLDiscountsSuccessfullyAndReturnDescriptiveMessage_WhenAllDiscountsAreValid()
        {
            var storeGuid = Guid.NewGuid();
            var s = new Store {Guid = storeGuid, StoreName = "Sarah Mashkaot"};
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
                simplePolicy);

            var buyer = new User { DateOfBirth = DateTime.Now.AddYears(-25), UserName = "matureBenny" };

            var ppList = new List<PurchaseProduct>();

            var compositeDiscount = new CompositeDiscount(new SumDiscountOperator());

            compositeDiscount.AddToDiscountList(twentyPercentOff);

            compositeDiscount.AddToDiscountList(tenPercentOff);

            compositeDiscount.AddToDiscountList(fivePercentOff);

            var pp = new PurchaseProduct(product, 10, 100, 100); ;

            ppList.Add(pp);

            var sp = new StorePurchase { PurchaseProducts = ppList, Buyer = buyer, Store = s, TotalPrice = 100, DiscountedPrice = 100};

            var spList = new List<StorePurchase> {sp};


            var purchase = new Purchase { StorePurchases = spList, TotalPrice = 100, DiscountedPrice = 68.4m, Buyer = buyer};

            sp.TotalPrice.Should().Be(100);

            var result = compositeDiscount.ApplyDiscount(sp);

            _testOutputHelper.WriteLine(result);

            sp.DiscountedPrice.Should().Be((decimal)68.4);
        }
    }


}
