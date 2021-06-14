using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BoomaEcommerce.Services.DTO;
using BoomaEcommerce.Services.Settings;
using BoomaEcommerce.Services.UseCases;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json.Linq;

namespace BoomaEcommerce.Api
{
    public class UseCaseActionCreationConverter : JsonCreationConverter<IUseCaseAction>
    {
        private readonly JwtSettings _jwtSettings;
        private readonly IServiceProvider _serviceProvider;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public UseCaseActionCreationConverter(JwtSettings jwtSettings, IServiceProvider serviceProvider,
            IHttpContextAccessor httpContextAccessor)
        {
            _jwtSettings = jwtSettings;
            _serviceProvider = serviceProvider;
            _httpContextAccessor = httpContextAccessor;
        }
        protected override IUseCaseAction Create(Type objectType, JObject jObject)
        {
            var type = jObject["Type"].ToObject<string>();
            return type switch
            {
                "LoginAction" => new LoginUseCaseAction(_jwtSettings, _serviceProvider, _httpContextAccessor),
                "CreateStoreAction" => new CreateStoreUseCaseAction(_serviceProvider, _httpContextAccessor),
                "GetOwnershipAction" => new GetOwnershipUseCaseAction(_serviceProvider, _httpContextAccessor),
                "NominateOwnerAction" => new NominateOwnerUseCaseAction(_serviceProvider, _httpContextAccessor),
                "GetStoresAction" => new GetStoresUseCaseAction(_serviceProvider, _httpContextAccessor),
                "RegisterAction" => new RegisterUseCaseAction(_jwtSettings, _serviceProvider, _httpContextAccessor),
                "CreateProductAction" => new CreateStoreProductUseCaseAction(_serviceProvider, _httpContextAccessor),
                "NominateManagerAction" => new NominateManagerUseCaseAction(_serviceProvider, _httpContextAccessor),
                "GetAllSellersAction" => new GetAllSellersUseCaseAction(_serviceProvider, _httpContextAccessor),
                "DeleteProductAction" => new DeleteProductUseCaseAction(_serviceProvider, _httpContextAccessor),
                "GetProductsAction" => new GetProductsUseCaseAction(_serviceProvider, _httpContextAccessor),
                "GetPolicyAction" => new GetPolicyUseCaseAction(_serviceProvider, _httpContextAccessor),
                "GetSuboridinatesAction" => new GetSubOridinatesUseCaseAction(_serviceProvider, _httpContextAccessor),
                "UpdateProductAction" => new UpdateProductUseCaseAction(_serviceProvider, _httpContextAccessor),
                "GetStoreAction" => new GetStoreUseCaseAction(_serviceProvider, _httpContextAccessor),
                "GetStorePurchaseHistoryAction" => new StorePurchaseHistoryUseCaseAction(_serviceProvider, _httpContextAccessor),
                "DeleteStoreAction" => new DeleteStoreUseCaseAction(_serviceProvider, _httpContextAccessor),
                "GetManagementAction" => new GetManagementUseCaseAction(_serviceProvider, _httpContextAccessor),
                "UpdatePermissionsAction" => new UpdatePermissionsUseCaeAction(_serviceProvider, _httpContextAccessor),
                "RemoveStoreOwnerAction" => new RemoveStoreOwnerUseCaseAction(_serviceProvider, _httpContextAccessor),
                "AddDiscountAction" => new AddDiscountUseCaseAction(_serviceProvider, _httpContextAccessor),
                "RemoveManagerAction" => new RemoveManagerUseCaseAction(_serviceProvider, _httpContextAccessor),
                "AddPolicyAction" => new AddPolicyUseCaseAction(_serviceProvider, _httpContextAccessor),
                "CreateDiscountSubPolicyAction" => new RemoveManagerUseCaseAction(_serviceProvider, _httpContextAccessor),
                "CreateDiscountPolicyAction" => new CreateDiscountPolicyUseCaseAction(_serviceProvider, _httpContextAccessor),
                "CreateDiscountAction" => new CreateDiscountUseCaseAction(_serviceProvider, _httpContextAccessor),
                "CreatePurchasePolicyAction" => new CreatePurchasePolicyUseCaseAction(_serviceProvider, _httpContextAccessor),
                "DeleteDiscountPolicyAction" => new DeleteDiscountPolicyUseCaseAction(_serviceProvider, _httpContextAccessor),
                "DeleteDiscountAction" => new DeleteDiscountUseCaseAction(_serviceProvider, _httpContextAccessor),
                "DeletePolicyAction" => new DeletePolicyUseCaseAction(_serviceProvider, _httpContextAccessor),
                "GetDiscountPolicyAction" => new GetDiscountPolicyUseCaseAction(_serviceProvider, _httpContextAccessor),
                "CreatePurchaseAction" => new CreatePurchaseUseCaseAction(_serviceProvider, _httpContextAccessor),
                "GetProductsFromStoreAction" => new GetProductsFromStoreUseCaseAction(_serviceProvider, _httpContextAccessor),
                "GetAllProductsAction" => new GetAllProductsUseCaseAction(_serviceProvider, _httpContextAccessor),
                "GetAllUserPurchaseHistoryAction" => new GetAllUserPurchaseHistoryUseCaseAction(_serviceProvider, _httpContextAccessor),
                "GetProductsByCategory" => new GetProductByCategoryUseCaseAction(_serviceProvider, _httpContextAccessor),
                "GetProductsByName" => new GetProductByNameUseCaseAction(_serviceProvider, _httpContextAccessor),
                "GetProductsByKeyWord" => new GetProductsByKeywordUseCaseAction(_serviceProvider, _httpContextAccessor),
                "GetProductAction" => new GetProductUseCaseAction(_serviceProvider, _httpContextAccessor),
                "GetPurchaseFinalPriceAction" => new GetPurchaseFinalPriceUseCaseAction(_serviceProvider, _httpContextAccessor),
                "AddPurchaseProductToShoppingBasketAction" => new AddPurchaseProductToShoppingBasketUseCaseAction(_serviceProvider, _httpContextAccessor),
                "CreateShoppingBasketAction" => new CreateShoppingBasketUseCaseAction(_serviceProvider, _httpContextAccessor),
                "DeletePurchaseProductFromShoppingBasketAction" => new DeletePurchaseProductFromShoppingBasketUseCaseAction(_serviceProvider, _httpContextAccessor),
                "DeleteShoppingBasketAction" => new DeleteShoppingBasketUseCaseAction(_serviceProvider, _httpContextAccessor),
                "GetBasicUserInfoAction" => new GetBasicUserInfoUseCaseAction(_serviceProvider, _httpContextAccessor),
                "GetShoppingCartAction" => new GetShoppingCartUseCaseAction(_serviceProvider, _httpContextAccessor),
                "GetUserInfoAction" => new GetUserInfoUseCaseAction(_serviceProvider, _httpContextAccessor),
                "SetNotificationAsSeenAction" => new SetNotificationsAsSeenUseCaseAction(_serviceProvider, _httpContextAccessor),
                "UpdateUserInfoAction" => new UpdateUserInfoUseCaseAction(_serviceProvider, _httpContextAccessor),
                "LoadTestAction" => new LoadTestUseCaseAction(_serviceProvider, _httpContextAccessor),

                _ => throw new ArgumentOutOfRangeException()
            };
        }
    }
}
