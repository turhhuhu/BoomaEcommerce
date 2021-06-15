using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using BoomaEcommerce.Api.Authorization;
using BoomaEcommerce.Data;
using BoomaEcommerce.Data.InMemory;
using BoomaEcommerce.Services.Authentication;
using BoomaEcommerce.Services.External;
using BoomaEcommerce.Services.External.Payment;
using BoomaEcommerce.Services.External.Supply;
using BoomaEcommerce.Services.Products;
using BoomaEcommerce.Services.Purchases;
using BoomaEcommerce.Services.Settings;
using BoomaEcommerce.Services.Stores;
using BoomaEcommerce.Services.Users;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Moq;

namespace BoomaEcommerce.Api
{
    public static class ServiceCollectionsExtension
    {
        public static IServiceCollection AddStoresService(this IServiceCollection services)
        {
            services.AddTransient<StoresService>();
            services.AddTransient<IStoresService, SecuredStoreService>(sp =>
            {
                var storeService = sp.GetService<StoresService>();
                var claims = sp.GetService<ClaimsPrincipal>();
                return new SecuredStoreService(claims, storeService);
            });
            return services;
        }

        public static IServiceCollection AddUsersService(this IServiceCollection services)
        {
            services.AddTransient<UsersService>();
            services.AddTransient<IUsersService, SecuredUserService>(sp =>
            {
                var userService = sp.GetService<UsersService>();
                var claims = sp.GetService<ClaimsPrincipal>();
                return new SecuredUserService(claims, userService);
            });
            return services;
        }

        public static IServiceCollection AddPurchasesService(this IServiceCollection services,
            IConfiguration configuration)
        {
            if (configuration.GetSection("UseStubExternalSystems").Get<bool>())
            {
                services.AddTransient(_ => Mock.Of<IPaymentClient>());
                services.AddTransient(_ => Mock.Of<ISupplyClient>());
            }
            else
            {
                services.AddTransient<IPaymentClient, PaymentClient>();
                services.AddTransient<ISupplyClient, SupplyClient>();
                services.AddHttpClient("externalClient", x => x.BaseAddress = new Uri("https://cs-bgu-wsep.herokuapp.com/"));
            }

            services.AddTransient<PurchasesService>();
            services.AddTransient<IPurchasesService, SecuredPurchaseService>(sp =>
            {
                var purchaseService = sp.GetService<PurchasesService>();
                var claims = sp.GetService<ClaimsPrincipal>();
                return new SecuredPurchaseService(claims, purchaseService);
            });
            return services;
        }
        public static IServiceCollection AddProductsService(this IServiceCollection services)
        {
            services.AddTransient<IProductsService, ProductsService>();
            var mistakeCorrectionMock = new Mock<IMistakeCorrection>();
            mistakeCorrectionMock.Setup(x => x.CorrectMistakeIfThereIsAny(It.IsAny<string>()))
                .Returns<string>(x => x);

            services.AddTransient(_ => mistakeCorrectionMock.Object);
            return services;
        }
        public static IServiceCollection AddTokenAuthentication(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<JwtSettings>(configuration.GetSection(JwtSettings.Section));


            var tokenValidationParams = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(configuration["Jwt:Secret"])),
                ValidateLifetime = true,
                ValidateAudience = false,
                ValidateIssuer = false,
                ClockSkew = TimeSpan.Zero
            };

            services.AddAuthentication(x =>
                {
                    x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    x.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                    x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                .AddJwtBearer(x =>
                {
                    x.SaveToken = true;
                    x.TokenValidationParameters = tokenValidationParams;
                });

            services.AddScoped<IAuthorizationHandler, TokenHasUserGuidAuthorizationHandler>();
            services.AddAuthorization(options =>
            {
                options.AddPolicy("TokenHasUserGuidPolicy", policy =>
                {
                    policy.Requirements.Add(new TokenHasUserGuidRequirement());
                });
            });
            services.AddTransient<IAuthenticationService, AuthenticationService>();

            services.AddTransient(_ => tokenValidationParams);


            services.AddHttpContextAccessor();

            services.AddTransient(s =>
                s.GetService<IHttpContextAccessor>()?.HttpContext?.User);

            return services;
        }

    }
}
