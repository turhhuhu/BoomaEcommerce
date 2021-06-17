using System;
using BoomaEcommerce.Core;

namespace BoomaEcommerce.Domain
{
    public class Notification : BaseEntity
    {
        public string Message { get; set; }
        public bool WasSeen { get; set; }

        public Notification(string message)
        {
            Message = message;
            WasSeen = false;
        }

        protected Notification()
        {
            Message = "";
            WasSeen = false;
        }
    }

    public class NewOfferNotification : Notification
    {
        public ProductOffer.ProductOffer Offer { get; set; }

        public NewOfferNotification(ProductOffer.ProductOffer offer, 
            string message = "A New offer has been received.") : base(message)
        {
            Offer = offer;
        }

        private NewOfferNotification() : base()
        { }
    }


    public class OfferDeclinedNotification : Notification
    {
        public ProductOffer.ProductOffer Offer { get; set; }

        public OfferDeclinedNotification(ProductOffer.ProductOffer offer,
            string message = "Your offer has been declined.") : base(message)
        {
            Offer = offer;
        }

        private OfferDeclinedNotification() : base()
        { }
    }

    public class OfferApprovedNotification : Notification
    {
        public ProductOffer.ProductOffer Offer { get; set; }

        public OfferApprovedNotification(ProductOffer.ProductOffer offer,
            string message = "Your offer has been approved.") : base(message)
        {
            Offer = offer;
        }

        private OfferApprovedNotification() : base()
        { }
    }

    public class StorePurchaseNotification : Notification
    {
        public User Buyer { get; set; }
        public Store Store { get; set; }
        public Guid StorePurchaseGuid { get; set; }

        public StorePurchaseNotification(User buyer, Guid storePurchaseGuid, Store store, string message = "A store purchase has been made.")
            : base(message)
        {
            Buyer = buyer;
            StorePurchaseGuid = storePurchaseGuid;
            Store = store;
        }

        private StorePurchaseNotification() : base()
        {}
    }
    public class RoleDismissalNotification : Notification
    {
        public User DismissingUser { get; set; }
        public Store Store { get; set; }


        public RoleDismissalNotification(User dismissingUser, Store store,
            string message = "You've been dismissed from an ownership store role.") : base(message)
        {
            DismissingUser = dismissingUser;
            Store = store;
        }

        private RoleDismissalNotification() : base()
        { }
    }
}
