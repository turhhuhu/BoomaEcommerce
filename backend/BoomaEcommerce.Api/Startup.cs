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
            services.AddCors(options =>
            {
                options.AddDefaultPolicy(
                    builder =>
                    {
                        builder
                            .AllowAnyHeader()
                            .AllowAnyMethod()
                            .AllowAnyOrigin();
                    });
            });

            services.AddControllers();

            services.AddMvc().AddJsonOptions(x => x.JsonSerializerOptions.IgnoreNullValues = true);

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
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "BoomaEcommerce.Api v1"));
                app.UseMiddleware<LoggingMiddleware>();
            }
            app.UseMiddleware<ExceptionsMiddleware>();

            app.UseHttpsRedirection();

            app.UseRouting();
            app.UseCors();
            app.UseAuthentication();
            app.UseAuthorization();



            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
