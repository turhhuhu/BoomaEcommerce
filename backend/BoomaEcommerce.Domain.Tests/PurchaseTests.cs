﻿using System.Threading.Tasks;
using AutoFixture;
using FluentAssertions;
using Xunit;

namespace BoomaEcommerce.Domain.Tests
{
    public class PurchaseTests
    {
        public PurchaseTests()
        {
            
        }
        
        [Fact]
        public async Task MakePurchase_ReturnsTrue_WhenStorePurchasesAreValid()
        {
            var sut = new Purchase {StorePurchases = TestData.GetTestValidStorePurchases(), TotalPrice = 450};

            var result = await sut.MakePurchase();

            result.Success.Should().BeTrue();
        }
        
        [Fact]
        public async Task MakePurchase_ReturnsFalse_WhenStorePurchasesAreInvalid()
        {
            var sut = new Purchase {StorePurchases = TestData.GetTestInvalidStorePurchases(), TotalPrice = 450};

            var result = await sut.MakePurchase();

            result.Success.Should().BeFalse();
        }
        
        [Fact]
        public void ValidatePrice_ReturnsTrue_WhenPriceValid()
        {
            var sut = new Purchase {StorePurchases = TestData.GetTestValidStorePurchases(), TotalPrice = 450};
            var result = sut.ValidatePrice();

            result.Should().BeTrue();
        }
        
        [Fact]
        public void ValidatePrice_ReturnsFalse_WhenPriceInvalid()
        {
            var sut = new Purchase {StorePurchases = TestData.GetTestInvalidStorePurchases(), TotalPrice = 450};

            var result = sut.ValidatePrice();

            result.Should().BeFalse();
        }

    }
}