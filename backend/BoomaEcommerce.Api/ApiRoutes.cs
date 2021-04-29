using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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
        }

        public const string Me = "me";

        public static class Cart
        {
            public const string CartsBase = "cart";
            public const string GetMe = CartsBase + "/" + Me;

            public static class Baskets
            {
                public const string BasketsBase = CartsBase + "/baskets";
                public const string PostMe = CartsBase + "/" + BasketsBase + "/" + Me;

                public static class PurchaseProducts
                {
                    public const string Post = BasketsBase + "/{basketGuid}" + "/products";
                }

            }
        }
    }
}
