using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using Xunit;

namespace BoomaEcommerce.Domain.Tests
{
    public class ProductOfferTests
    {
        [Fact]
        public void DeclineOffer_ShouldChangeOfferState_WhenStateIsPending()
        {

            //Arrange
            var p = new ProductOffer.ProductOffer(null)
            { State = ProductOffer.ProductOfferState.Pending };

            //Act
            p.DeclineOffer(null);

            //Assert
            p.State.Should().Be(ProductOffer.ProductOfferState.Declined);
        }

        [Fact]
        public void DeclineOffer_ShouldNotChangeOfferState_WhenStateIsNotPending()
        {

            //Arrange
            var p = new ProductOffer.ProductOffer(null)
            { State = ProductOffer.ProductOfferState.Approved };

            //Act
            p.DeclineOffer(null);

            //Assert
            p.State.Should().Be(ProductOffer.ProductOfferState.Approved);
        }

        [Fact]
        public void ApproveOffer_ShouldAddOwnerToApproversAndNotChangeState_WhenApproversNotContainsStoreOwners()
        {
            //Arrange
            var o1 = new StoreOwnership { };
            var o2 = new StoreOwnership { };
            var o3 = new StoreOwnership { };
            var ownersInStore = new List<StoreOwnership>();
            ownersInStore.Add(o1);
            ownersInStore.Add(o2);
            ownersInStore.Add(o3);

            var a1 = new ProductOffer.ApproverOwner { Approver = o1 };
            var approvers = new List<ProductOffer.ApproverOwner>();
            approvers.Add(a1);

            var p = new ProductOffer.ProductOffer(null)
            { State = ProductOffer.ProductOfferState.Pending, ApprovedOwners = approvers };

            //Act
            p.ApproveOffer(o2, ownersInStore);

            //Assert
            p.State.Should().Be(ProductOffer.ProductOfferState.Pending);
            p.ApprovedOwners.Should().Contain(approver => approver.Approver == o2);
        }


        [Fact]
        public void ApproveOffer_ShouldAddOwnerToApproversAndChangeState_WhenApproversContainsStoreOwners()
        {
            //Arrange
            var o1 = new StoreOwnership { };
            var o2 = new StoreOwnership { };
            var o3 = new StoreOwnership { };
            var ownersInStore = new List<StoreOwnership>();
            ownersInStore.Add(o1);
            ownersInStore.Add(o2);
            ownersInStore.Add(o3);

            var a1 = new ProductOffer.ApproverOwner { Approver = o1 };
            var a2 = new ProductOffer.ApproverOwner { Approver = o2 };

            var approvers = new List<ProductOffer.ApproverOwner>();
            approvers.Add(a1);
            approvers.Add(a2);


            var p = new ProductOffer.ProductOffer(null)
            { State = ProductOffer.ProductOfferState.Pending, ApprovedOwners = approvers };

            //Act
            p.ApproveOffer(o3, ownersInStore);

            //Assert
            p.State.Should().Be(ProductOffer.ProductOfferState.Approved);
            p.ApprovedOwners.Should().Contain(approver => approver.Approver == o3);
        }

        [Fact]
        public void MakeCounterOffer_ShouldNotChangeOfferPrice_WhenCounterOfferAlreadyRecieved()
        {
            //Arrange
            var p = new ProductOffer.ProductOffer(null)
            { State = ProductOffer.ProductOfferState.CounterOfferReceived, CounterOfferPrice = 5 };

            //Act
            p.MakeCounterOffer(3);

            //Assert
            p.State.Should().Be(ProductOffer.ProductOfferState.CounterOfferReceived);
            p.CounterOfferPrice.Should().Be(5);
        }

        [Fact]
        public void MakeCounterOffer_ShouldChangeOfferPrice_WhenOfferStateIsPending()
        {
            //Arrange
            var p = new ProductOffer.ProductOffer(null)
            { State = ProductOffer.ProductOfferState.Pending };

            //Act
            p.MakeCounterOffer(3);

            //Assert
            p.State.Should().Be(ProductOffer.ProductOfferState.CounterOfferReceived);
            p.CounterOfferPrice.Should().Be(3);
        }

    }
}

