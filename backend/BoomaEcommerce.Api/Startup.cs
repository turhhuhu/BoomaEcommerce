using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using BoomaEcommerce.Api.Middleware;
using BoomaEcommerce.Data.InMemory;
using BoomaEcommerce.Domain;
using BoomaEcommerce.Services.Authentication;
using BoomaEcommerce.Services.Settings;
using BoomaEcommerce.Services.Stores;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using BoomaEcommerce.Data;
using BoomaEcommerce.Services;
using BoomaEcommerce.Services.External;
using BoomaEcommerce.Services.MappingProfiles;
using BoomaEcommerce.Services.Products;
using BoomaEcommerce.Services.Users;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Moq;
using Serilog;
using Microsoft.AspNetCore.Http.Connections;
using BoomaEcommerce.Api.Hubs;
using Microsoft.AspNetCore.SignalR;
using BoomaEcommerce.Api.Authorization;
using Microsoft.AspNetCore.Authorization;
namespace BoomaEcommerce.Api
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddCors();

            services.AddControllers();

            services.AddMvc().AddNewtonsoftJson().AddJsonOptions(x => x.JsonSerializerOptions.IgnoreNullValues = true);

            services.AddSignalR(hubOptions =>
            {
                hubOptions.EnableDetailedErrors = true;
                hubOptions.KeepAliveInterval = TimeSpan.FromMinutes(1);
                hubOptions.AddFilter<ExceptionHandlingFilter>();
            });

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "BoomaEcommerce.Api", Version = "v1" });
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Description = "JWT Authorization header using bearer scheme",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey,
                    BearerFormat = "JWT",
                });
                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        Array.Empty<string>()
                    }
                });
            });

            services.AddSingleton<AppInitializer>();

            services.Configure<AppInitializationSettings>(Configuration.GetSection(AppInitializationSettings.Section));
            services.Configure<JwtSettings>(Configuration.GetSection(JwtSettings.Section));
            services.AddSingleton(_ => new UserManager<User>(
                new InMemoryUserStore(), Options.Create
                    (new IdentityOptions
                    {
                        Stores = new StoreOptions { ProtectPersonalData = false }
                    }), new PasswordHasher<User>(), new IUserValidator<User>[0], new IPasswordValidator<User>[0], new UpperInvariantLookupNormalizer(), new IdentityErrorDescriber(), new DataColumn(), new Logger<UserManager<User>>(new LoggerFactory())));

            services.AddAutoMapper(typeof(DomainToDtoProfile), typeof(DtoToDomainProfile));

            var tokenValidationParams = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(Configuration["Jwt:Secret"])),
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

            services.AddSingleton<IAuthorizationHandler, TokenHasUserGuidAuthorizationHandler>();
            services.AddAuthorization(options =>
            {
                options.AddPolicy("TokenHasUserGuidPolicy", policy =>
                {
                    policy.Requirements.Add(new TokenHasUserGuidRequirement());
                });
            });

            services.AddSingleton(typeof(IRepository<>), typeof(InMemoryRepository<>));
            services.AddTransient(_ => tokenValidationParams);
            services.AddSingleton<IAuthenticationService, AuthenticationService>();

            services.AddSingleton<IStoreUnitOfWork, InMemoryStoreUnitOfWork>();
            services.AddSingleton<StoresService>();
            services.AddSingleton<IProductsService, ProductsService>();

            services.AddHttpContextAccessor();

            services.AddTransient(s =>
                s.GetService<IHttpContextAccessor>()?.HttpContext?.User);

            services.AddTransient<IStoresService, SecuredStoreService>(sp =>
            {
                var storeService = sp.GetService<StoresService>();
                var claims = sp.GetService<ClaimsPrincipal>();
                return new SecuredStoreService(claims, storeService);
            });

            services.AddSingleton<IUserUnitOfWork, InMemoryUserUnitOfWork>();
            services.AddSingleton<UsersService>();
            services.AddTransient<IUsersService, SecuredUserService>(sp =>
            {
                var userService = sp.GetService<UsersService>();
                var claims = sp.GetService<ClaimsPrincipal>();
                return new SecuredUserService(claims, userService);
            });

            var mistakeCorrectionMock = new Mock<IMistakeCorrection>();
            mistakeCorrectionMock.Setup(x => x.CorrectMistakeIfThereIsAny(It.IsAny<string>()))
                .Returns<string>(x => x);
            services.AddSingleton(_ => mistakeCorrectionMock.Object);
            services.AddSingleton<INotificationHub, NotificationHub>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "BoomaEcommerce.Api v1"));
                
            }

            app.UseCors(builder => builder
                .AllowAnyMethod()
                .AllowAnyHeader()
                .AllowCredentials()
                .SetIsOriginAllowed(origin => true));

            app.UseMiddleware<WebSocketsMiddleware>();
            app.UseMiddleware<ExceptionsMiddleware>();

            app.UseAuthentication();
            app.UseRouting();
            app.UseAuthorization();
            //app.UseHttpsRedirection();


            app.UseSerilogRequestLogging();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapHub<NotificationHub>("/hub/notifications", options =>
                {
                    options.Transports = HttpTransportType.WebSockets;
                });
            });
        }
    }
}
