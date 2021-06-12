﻿using System;
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
        public List<StoreOwnership> ApprovedOwners { get; set; }

        public ProductOfferState CheckProductOfferState(List<StoreOwnership> ownersInStore)
        {
            if (ownersInStore.Select(o => o.Guid).Any(guid => ApprovedOwners.Select(o => o.Guid).Contains(guid)))
            {
                // Owners in store all contained in the approval list
                this.State = ProductOfferState.Approved;
                return ProductOfferState.Approved;
            }

            return ProductOfferState.Pending;
        }

        public ProductOfferState ApproveOffer(StoreOwnership owner, List<StoreOwnership> ownersInStore)
        {
            if(this.State == ProductOfferState.Pending) { 
                this.ApprovedOwners.Add(owner);
                return CheckProductOfferState(ownersInStore);
            }

            return ProductOfferState.Pending;
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
        Error
    }
}
