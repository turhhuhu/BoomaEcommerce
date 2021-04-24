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
using System.Text;
using System.Threading.Tasks;
using BoomaEcommerce.Data.InMemory;
using BoomaEcommerce.Domain;
using BoomaEcommerce.Services.Authentication;
using BoomaEcommerce.Services.Settings;
using BoomaEcommerce.Services.Stores;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using BoomaEcommerce.Data;
using BoomaEcommerce.Services.External;
using BoomaEcommerce.Services.MappingProfiles;
using BoomaEcommerce.Services.Products;
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

            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "BoomaEcommerce.Api", Version = "v1" });
            });

            services.Configure<JwtSettings>(Configuration.GetSection("Jwt"));

            services.AddSingleton(_ => new UserManager<User>(
                new InMemoryUserStore(), Options.Create
                    (new IdentityOptions
                    {
                        Stores = new StoreOptions { ProtectPersonalData = false }
                    }), new PasswordHasher<User>(), new IUserValidator<User>[0], new IPasswordValidator<User>[0], new UpperInvariantLookupNormalizer(), new IdentityErrorDescriber(), new DataColumn(), new Logger<UserManager<User>>(new LoggerFactory())));

            //services.AddSingleton<InMemoryUserStore>();
            //services.AddSingleton<UserManager<User>, InMemoryUserManager>();

            services.AddAutoMapper(typeof(DomainToDtoProfile), typeof(DtoToDomainProfile));

            var tokenValidationParams = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(Configuration.GetValue<string>("Jwt:Secret"))),
                ValidateLifetime = true,
                ValidateAudience = false,
                ValidateIssuer = false,
                ClockSkew = TimeSpan.Zero
            };

            services.AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
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
            services.AddSingleton<IStoresService, StoresService>();
            services.AddSingleton<IProductsService, ProductsService>();
            services.AddSingleton(_ => new Mock<IMistakeCorrection>().Object);
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

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }

    public class ApplicationRole
    {
    }
}
