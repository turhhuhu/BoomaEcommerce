using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BoomaEcommerce.Core;

namespace BoomaEcommerce.Domain.ProductOffer
{
    public class ProductOffer : BaseEntity
    {
        public User User { get; set; }
        public Product Product { get; set; }
        public ProductOfferState State { get; set; }
        public decimal OfferPrice { get; set; }
        public decimal? CounterOfferPrice { get; set; }
        public List<ApproverOwner> ApprovedOwners { get; set; }

        public ProductOfferState CheckProductOfferState(List<StoreOwnership> ownersInStore)
        {
            if (OwnersInStoreContainedInApprovers(ownersInStore))
            {
                // Owners in store all contained in the approval list
                this.State = ProductOfferState.Approved;
                return ProductOfferState.Approved;
            }

            return ProductOfferState.Pending;
        }

        private bool OwnersInStoreContainedInApprovers(List<StoreOwnership> ownersInStore)
        { 
            foreach(var owner in ownersInStore)
            {
                if (!(this.ApprovedOwners.Select(o=>o.Approver.Guid).Contains(owner.Guid)))
                {
                    return false;
                }
            }
            return true;
        }
        public ApproverOwner ApproveOffer(StoreOwnership owner, List<StoreOwnership> ownersInStore)
        {
            ApproverOwner approver = null;
            if (this.State == ProductOfferState.Pending) {
                approver = new ApproverOwner() { Approver = owner };
                this.ApprovedOwners.Add(approver);
                CheckProductOfferState(ownersInStore);
            }

            return approver;
        }

        public void DeclineOffer(StoreOwnership owner)
        {
            if (this.State == ProductOfferState.Pending)
            {
                this.State = ProductOfferState.Declined;
            }
        }

        public PurchaseProduct ConvertOfferToPurchaseProduct()
        {
            var pp = new PurchaseProduct()
            {
                Price = this.OfferPrice,
                Amount = 1,
                Product = this.Product
            };

            return pp;
        }

        public void MakeCounterOffer(decimal counterOfferPrice)
        {
            if (this.State == ProductOfferState.CounterOfferReceived) return;
            this.State = ProductOfferState.CounterOfferReceived;
            this.CounterOfferPrice = counterOfferPrice;
        }

        public ProductOffer(User user)
        {
            this.State = ProductOfferState.Pending;
            this.ApprovedOwners = new List<ApproverOwner>();
            this.OfferPrice = 0;
            this.CounterOfferPrice = 0;
            this.User = user;
        }

        private ProductOffer()
        {
        }


    }

    // 1. CreateProductOffer(Guid userGuid, Product product, decimal price)  
    // 2. ApproveOffer(Guid ownerGuid, Guid productOfferGuid)
    // 2b. DeclinesOffer(Guid ownerGuid, Guid productOfferGuid)
    // 3. Already exist - Remove Ownership
    // 4. makeCounterOffer(Guid ownerGuid, decimal counterOfferPrice)
    // 5. makeOfferToPurchaseProduct(Guid userGuid)



    public enum ProductOfferState
    {
        Pending,
        Approved,
        Declined,
        CounterOfferReceived,
        Error,
        Purchased
    }
}
