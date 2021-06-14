using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BoomaEcommerce.Services.DTO
{
    public class NotificationDto : BaseEntityDto
    {
        public virtual NotificationType Type { get; set; } = NotificationType.Notification;
        public string Message { get; set; }
        public bool WasSeen { get; set; }
    }
    public class StorePurchaseNotificationDto : NotificationDto
    {
        public override NotificationType Type { get; set; } = NotificationType.StorePurchaseNotification;
        public UserMetaData BuyerMetaData { get; set; }
        public StoreMetaData StoreMetaData { get; set; }
        public Guid StorePurchaseGuid { get; set; }
    }

    public class NewOfferNotificationDto : NotificationDto
    {
        public override NotificationType Type { get; set; } = NotificationType.NewOfferNotification;

        public UserMetaData OfferUserMetaData { get; set; }

        public ProductMetaData ProductMetaData { get; set; }
    }

    public class OfferDeclinedNotificationDto : NotificationDto
    {
        public override NotificationType Type { get; set; } = NotificationType.OfferDeclinedNotification;

        public UserMetaData OfferUserMetaData { get; set; }

        public ProductMetaData ProductMetaData { get; set; }
    }

    public class OfferApprovedNotificationDto : NotificationDto
    {
        public override NotificationType Type { get; set; } = NotificationType.OfferApprovedNotification;

        public UserMetaData OfferUserMetaData { get; set; }

        public ProductMetaData ProductMetaData { get; set; }
    }

    public class RoleDismissalNotificationDto : NotificationDto
    {
        public override NotificationType Type { get; set; } = NotificationType.RoleDismissalNotification;
        public UserMetaData DismissingUserMetaData { get; set; }
        public StoreMetaData StoreMetaData { get; set; }
    }
    public enum NotificationType
    {
        Notification,
        StorePurchaseNotification,
        RoleDismissalNotification,
        NewOfferNotification,
        OfferDeclinedNotification,
        OfferApprovedNotification
    }
}
