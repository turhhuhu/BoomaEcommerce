using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BoomaEcommerce.Domain.PurchasePolicy;
using BoomaEcommerce.Domain.PurchasePolicy.Operators;
using BoomaEcommerce.Domain.PurchasePolicy.Policies;
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
            var andComposite = new CompositePurchasePolicy(new AndPolicyOperator());
            andComposite.AddPolicy(new AgeRestrictionPolicy(product, 18));
            andComposite.AddPolicy(new MaxProductAmountPolicy(product, 10));

            var childAndComposite = new CompositePurchasePolicy(new AndPolicyOperator());

            andComposite.AddPolicy(childAndComposite);
            childAndComposite.AddPolicy(new MinProductAmountPolicy(product, 9));

            var shoppingBasket = new ShoppingBasket();
            shoppingBasket.AddPurchaseProduct(new PurchaseProduct(product, 8, 10));
            var result = andComposite.CheckPolicy(new User {DateOfBirth = DateTime.Now.AddYears(-10), UserName = "arik1337"}, shoppingBasket);
            _testOutputHelper.WriteLine(result.PolicyError);
        }
    }


}

