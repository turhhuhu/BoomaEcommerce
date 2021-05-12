using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BoomaEcommerce.Domain;

namespace BoomaEcommerce.Api
{
    public static class ApiRoutes
    {
        public static class Auth
        {
            public const string Register = "register";

            public const string Login = "login";

            public const string Refresh = "refresh";
        }
        public static class Products
        {
            public const string ProductsBase = "products";
            public const string Get = "{productGuid}";
            public const string Post = ProductsBase;
            public const string Delete = "{productGuid}";
            public const string Put = ProductsBase;
        }

        public const string Me = "me";
        public static class Notifications
        {
            public const string NotificationsBase = "notifications";
            public const string Post = "{userGuid}" + "/" + NotificationsBase + "/";
        }

        public static class Cart
        {
            public const string CartsMeBase = Me + "/cart";
            public const string MeGet = CartsMeBase;
            public static class Baskets
            {
                public const string BasketsMeBase = CartsMeBase + "/baskets";
                public const string MePost = BasketsMeBase;
                public const string Get = BasketsMeBase + "/{basketGuid}";
                public const string MeDelete = BasketsMeBase + "/{basketGuid}";
                public static class PurchaseProducts
                {
                    public const string PurchaseProductsMeBase = BasketsMeBase + "/{basketGuid}" + "/products";
                    public const string MePost = PurchaseProductsMeBase;
                    public const string MeDelete = PurchaseProductsMeBase + "/{purchaseProductGuid}";
                }

            }

        }

        public class Stores
        {
            public const string StoreGuid = "{storeGuid}";
            public const string StoresBase = "stores";
            public const string MePost = Me + "/" + StoresBase;
            public const string GetAllProducts = StoreGuid + "/" + Products.ProductsBase;
            public const string Get = StoreGuid;
            public const string AllRolesGet = Me + "/" + StoresBase + "/" + "allRoles";

            public class Roles
            {
                public const string RolesBase = "roles";
                public const string Get = StoreGuid + "/" + RolesBase;
                public const string MeRoleGet = Me + "/" + StoresBase + "/" + StoreGuid + "/" + "role";
                public class Ownerships
                {
                    public const string OwnershipsBase = "ownerships";
                    public const string Post = StoreGuid + "/" + RolesBase + "/" + OwnershipsBase;
                }

                public class Managements
                {
                    public const string ManagementsBase = "managements";
                    public const string Post = StoreGuid + "/" + RolesBase + "/" + ManagementsBase;
                }
            }
        }
    }
}
