using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BoomaEcommerce.Domain.Policies;
using BoomaEcommerce.Domain.Policies.Operators;
using BoomaEcommerce.Domain.Policies.PolicyTypes;
using FluentAssertions;
using Xunit;
using Xunit.Abstractions;

namespace BoomaEcommerce.Domain.Tests
{
    public class PurchasePolicyTests
    {
        private readonly ITestOutputHelper _testOutputHelper;

        public PurchasePolicyTests(ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;
        }

        [Fact]
        public void CheckPolicy_MustGetOkResultWithErrorMessage_WhenPolicyNotSatisfied()
        {
            var product = new Product
            {
                Name = "Milk",
                Amount = 20
            };
            var andComposite = new CompositePolicy(new AndPolicyOperator());
            andComposite.AddPolicy(new AgeRestrictionPolicy(product, 18));
            andComposite.AddPolicy(new MaxProductAmountPolicy(product, 10));

            var childAndComposite = new CompositePolicy(new AndPolicyOperator());

            andComposite.AddPolicy(childAndComposite);
            childAndComposite.AddPolicy(new MinProductAmountPolicy(product, 9));

            var shoppingBasket = new ShoppingBasket();
            shoppingBasket.AddPurchaseProduct(new PurchaseProduct(product, 8, 10));
            var result = andComposite.CheckPolicy(new User {DateOfBirth = DateTime.Now.AddYears(-15), UserName = "matureOri"}, shoppingBasket);
            _testOutputHelper.WriteLine(result.PolicyError);
        }

        [Fact]
        public void CheckPolicy_MustGetOkResult_WhenOneOfTheOrBranchesIsSatisfied()
        {
            var product = new Product
            {
                Name = "Beer",
                Amount = 20
            };
            var andComposite18 = new CompositePolicy(new AndPolicyOperator());
            andComposite18.AddPolicy(new AgeRestrictionPolicy(product, 18));
            andComposite18.AddPolicy(new MaxProductAmountPolicy(product, 6));

            var andComposite21 = new CompositePolicy(new AndPolicyOperator());
            andComposite21.AddPolicy(new AgeRestrictionPolicy(product, 21));
            andComposite21.AddPolicy(new MaxProductAmountPolicy(product, 10));


            var orCompositePolicy = new CompositePolicy(new OrPolicyOperator());

            orCompositePolicy.AddPolicy(andComposite18);
            orCompositePolicy.AddPolicy(andComposite21);

            var shoppingBasket = new ShoppingBasket();
            shoppingBasket.AddPurchaseProduct(new PurchaseProduct(product, 5, 10));
            var result = orCompositePolicy.CheckPolicy(new User { DateOfBirth = DateTime.Now.AddYears(-19), UserName = "YoungBenny" }, shoppingBasket);
        }

        [Fact]
        public void CheckPolicy_MustFail_WhenTheSuitableOrBranchesIsNOTSatisfied()
        {
            var product = new Product
            {
                Name = "Beer",
                Amount = 20
            };
            var andComposite18 = new CompositePolicy(new AndPolicyOperator());
            andComposite18.AddPolicy(new AgeRestrictionPolicy(product, 18));
            andComposite18.AddPolicy(new MaxProductAmountPolicy(product, 6));

            var andComposite21 = new CompositePolicy(new AndPolicyOperator());
            andComposite21.AddPolicy(new AgeRestrictionPolicy(product, 21));
            andComposite21.AddPolicy(new MaxProductAmountPolicy(product, 10));


            var orCompositePolicy = new CompositePolicy(new OrPolicyOperator());

            orCompositePolicy.AddPolicy(andComposite18);
            orCompositePolicy.AddPolicy(andComposite21);

            var shoppingBasket = new ShoppingBasket();
            shoppingBasket.AddPurchaseProduct(new PurchaseProduct(product, 7, 10));
            var result = orCompositePolicy.CheckPolicy(new User { DateOfBirth = DateTime.Now.AddYears(-19), UserName = "YoungBenny" }, shoppingBasket);
            result.IsOk.Should().Be(false);
            _testOutputHelper.WriteLine(result.PolicyError);
        }

        [Fact]
        public void CheckPolicy_MustReturnOKResult_WhenXORIsFulfilled()
        {
            var bluePillProduct = new Product
            {
                Name = "BluePill", // Matrix ;)
                Amount = 10
            };

            var redPillProduct = new Product
            {
                Name = "RedPill",  
                Amount = 12
            };


            var xorCompositeGrownups = new CompositePolicy(new XorPolicyOperator());

            var takeRedPill = new CompositePolicy(new AndPolicyOperator());
            takeRedPill.AddPolicy(new MinProductAmountPolicy(redPillProduct, 1));
            takeRedPill.AddPolicy(new MaxProductAmountPolicy(redPillProduct, 1));

            var takeBluePill = new CompositePolicy(new AndPolicyOperator());
            takeRedPill.AddPolicy(new MinProductAmountPolicy(bluePillProduct, 1));
            takeRedPill.AddPolicy(new MaxProductAmountPolicy(bluePillProduct, 1));

            xorCompositeGrownups.AddPolicy(takeBluePill);
            xorCompositeGrownups.AddPolicy(takeRedPill);

            var shoppingBasketBlue = new ShoppingBasket();
            shoppingBasketBlue.AddPurchaseProduct(new PurchaseProduct(bluePillProduct, 1, 10));
            var resultBlue = xorCompositeGrownups.CheckPolicy(new User { DateOfBirth = DateTime.Now.AddYears(-17), UserName = "YoungBenny" }, shoppingBasketBlue);
            resultBlue.IsOk.Should().Be(true);


            var shoppingBasketRed = new ShoppingBasket();
            shoppingBasketBlue.AddPurchaseProduct(new PurchaseProduct(redPillProduct, 1, 10));
            var resultRed = xorCompositeGrownups.CheckPolicy(new User { DateOfBirth = DateTime.Now.AddYears(-17), UserName = "YoungBenny" }, shoppingBasketRed);
            resultRed.IsOk.Should().Be(true);

            var shoppingBasketBothPills = new ShoppingBasket(); // Big No-No
            shoppingBasketBothPills.AddPurchaseProduct(new PurchaseProduct(redPillProduct, 1, 10));
            shoppingBasketBothPills.AddPurchaseProduct(new PurchaseProduct(bluePillProduct, 1, 10));
            var resultBoth = xorCompositeGrownups.CheckPolicy(new User { DateOfBirth = DateTime.Now.AddYears(-17), UserName = "YoungBenny" }, shoppingBasketBothPills);
            resultBoth.IsOk.Should().Be(false);
            _testOutputHelper.WriteLine(resultBoth.PolicyError);
        }

        [Fact]
        public void CheckPolicy_MustReturnFail_WhenOrIsNOTSatisfied()
        {
            var product = new Product
            {
                Name = "Very Strong Wine",
                Amount = 20
            };
            var orCompositePolicy = new CompositePolicy(new OrPolicyOperator());
            orCompositePolicy.AddPolicy(new AgeRestrictionPolicy(product, 21));
            orCompositePolicy.AddPolicy(new MaxProductAmountPolicy(product, 1));

            var shoppingBasket = new ShoppingBasket();
            shoppingBasket.AddPurchaseProduct(new PurchaseProduct(product, 2, 10));
            var result = orCompositePolicy.CheckPolicy(new User { DateOfBirth = DateTime.Now.AddYears(-19), UserName = "YoungBenny" }, shoppingBasket);
            result.IsOk.Should().Be(false);
            _testOutputHelper.WriteLine(result.PolicyError);
        }

        [Fact]
        public void CheckPolicy_MustReturnOK_WhenIfPolicyIsSatisfiedInConditionalAndThenPolicyIsFollowed()
        {
            var product = new Product
            {
                Name = "Vitamins For Adults",
                Amount = 20
            };
            var conditionalPolicy = new CompositePolicy(new ConditionPolicyOperator());
            conditionalPolicy.AddPolicy(new MinProductAmountPolicy(product, 1));
            conditionalPolicy.AddPolicy(new AgeRestrictionPolicy(product, 65));
            

            var shoppingBasket = new ShoppingBasket();
            shoppingBasket.AddPurchaseProduct(new PurchaseProduct(product, 2, 10));
            var result = conditionalPolicy.CheckPolicy(new User { DateOfBirth = DateTime.Now.AddYears(-66), UserName = "OldMatan" }, shoppingBasket);
            result.IsOk.Should().Be(true);
        }


        [Fact]
        public void CheckPolicy_BigTreeReturnsOk_WhenAgeAndAmountAreValidByPolicy()
        {
            var beerProduct = new Product
            {
                Name = "Beer",
                Amount = 20
            };

            var milkProduct = new Product
            {
                Name = "Milk",
                Amount = 20
            };

            var eggsProduct = new Product
            {
                Name = "Eggs",
                Amount = 20
            };


            // Big Policies Tree
            var rootOR = new CompositePolicy(new OrPolicyOperator());

            
            // If you buy beer 

            var andBeer = new CompositePolicy(new AndPolicyOperator());
            andBeer.AddPolicy(new AgeRestrictionPolicy(beerProduct, 18)); // You are 18 
            andBeer.AddPolicy(new MaxProductAmountPolicy(beerProduct, 6)); // and you buy at most 6 - beers 

            var orBeer = new CompositePolicy(new OrPolicyOperator());
            orBeer.AddPolicy(andBeer);
            orBeer.AddPolicy(new AgeRestrictionPolicy(beerProduct, 21)); // Or you are over 21

            var conditionalBeer = new BinaryPolicy(new ConditionPolicyOperator(), (new MinProductAmountPolicy(beerProduct, 1)), orBeer);

            rootOR.AddPolicy(conditionalBeer);

            var xorEggsAndMilk = new CompositePolicy(new XorPolicyOperator());

            var andMilkAgeRestriction = new CompositePolicy(new AndPolicyOperator()); 
            andMilkAgeRestriction.AddPolicy(new AgeRestrictionPolicy(milkProduct, 21)); // you have to be at least 21
            andMilkAgeRestriction.AddPolicy(new MinProductAmountPolicy(milkProduct, 5)); // and you have to buy at least 5 milk 

            var eggsAmountPolicy = new CompositePolicy(new AndPolicyOperator());
            eggsAmountPolicy.AddPolicy(new MaxProductAmountPolicy(eggsProduct, 10));
            eggsAmountPolicy.AddPolicy(new MinProductAmountPolicy(eggsProduct, 10)); // Or you buy exactly 10 eggs, but not both

            xorEggsAndMilk.AddPolicy(eggsAmountPolicy);
            xorEggsAndMilk.AddPolicy(andMilkAgeRestriction);

            rootOR.AddPolicy(xorEggsAndMilk);

            var shoppingBasket = new ShoppingBasket();
            shoppingBasket.AddPurchaseProduct(new PurchaseProduct(beerProduct, 7, 10));
            var result = rootOR.CheckPolicy(new User { DateOfBirth = DateTime.Now.AddYears(-22), UserName = "GrownOmer" }, shoppingBasket);
            result.IsOk.Should().Be(true);
        }

        [Fact]
        public void CheckPolicy_BigTreeFails_WhenProductAmountIsNOTValidByPolicy()
        {
            var beerProduct = new Product
            {
                Name = "Beer",
                Amount = 20
            };

            var milkProduct = new Product
            {
                Name = "Milk",
                Amount = 20
            };

            var eggsProduct = new Product
            {
                Name = "Eggs",
                Amount = 20
            };


            // Big Policies Tree
            var rootOR = new CompositePolicy(new OrPolicyOperator());


            // If you buy beer 

            var OrBeer = new CompositePolicy(new OrPolicyOperator());

            var youngBeer = new CompositePolicy(new AndPolicyOperator());

            youngBeer.AddPolicy(new AgeRestrictionPolicy(beerProduct, 18)); // You are 18 
            youngBeer.AddPolicy(new MaxProductAmountPolicy(beerProduct, 6)); // and you buy at most 6 - beers 

            var grownBeer = new CompositePolicy(new AndPolicyOperator());

            grownBeer.AddPolicy(new AgeRestrictionPolicy(beerProduct, 21)); // You are 21 
            grownBeer.AddPolicy(new MaxProductAmountPolicy(beerProduct, 18)); // and you buy at most 18 - beers 

            OrBeer.AddPolicy(youngBeer);
            OrBeer.AddPolicy(grownBeer);

            rootOR.AddPolicy(OrBeer);

            var EggsOrMilk = new CompositePolicy(new OrPolicyOperator());

            var andMilkAgeRestriction = new CompositePolicy(new AndPolicyOperator());
            andMilkAgeRestriction.AddPolicy(new AgeRestrictionPolicy(milkProduct, 21)); // you have to be at least 21
            andMilkAgeRestriction.AddPolicy(new MinProductAmountPolicy(milkProduct, 5)); // and you have to buy at least 5 milk 

            var eggsAmountPolicy = new CompositePolicy(new AndPolicyOperator());
            eggsAmountPolicy.AddPolicy(new MaxProductAmountPolicy(eggsProduct, 10));
            eggsAmountPolicy.AddPolicy(new MinProductAmountPolicy(eggsProduct, 10)); // Or you buy exactly 10 eggs

            EggsOrMilk.AddPolicy(eggsAmountPolicy);
            EggsOrMilk.AddPolicy(andMilkAgeRestriction);

            rootOR.AddPolicy(EggsOrMilk);

            var shoppingBasket = new ShoppingBasket();
            shoppingBasket.AddPurchaseProduct(new PurchaseProduct(eggsProduct, 15, 10));
            var result = rootOR.CheckPolicy(new User { DateOfBirth = DateTime.Now.AddYears(-12), UserName = "BabyArik" }, shoppingBasket);
            result.IsOk.Should().Be(false);

            _testOutputHelper.WriteLine(result.PolicyError);
        }

    }


}

