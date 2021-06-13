using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using System;
using System.Data;
using System.Net.Http;
using System.Text.Json.Serialization;
using BoomaEcommerce.Api.Config;
using BoomaEcommerce.Api.Middleware;
using BoomaEcommerce.Data.InMemory;
using BoomaEcommerce.Domain;
using BoomaEcommerce.Services.Settings;
using Microsoft.AspNetCore.Identity;
using BoomaEcommerce.Data;
using BoomaEcommerce.Services;
using BoomaEcommerce.Services.MappingProfiles;
using Microsoft.Extensions.Options;
using Serilog;
using Microsoft.AspNetCore.Http.Connections;
using BoomaEcommerce.Api.Hubs;
using BoomaEcommerce.Data.EfCore;
using BoomaEcommerce.Data.EfCore.Repositories;
using BoomaEcommerce.Domain.Policies;
using BoomaEcommerce.Services.External.Payment;
using BoomaEcommerce.Services.External.Supply;
using BoomaEcommerce.Services.SwaggerFilters;
using BoomaEcommerce.Services.UseCases;
using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;

namespace BoomaEcommerce.Api
{
    public class Startup
    {
        public string ConnectionString { get; set; }
        public DbMode DbMode { get; set; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
            ConnectionString = Configuration.GetConnectionString("DefaultConnectionString");
            DbMode = Configuration.GetSection("DbMode").Get<DbMode>();
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddCors();
                services.AddControllers();

            services.AddMvc()
                .AddNewtonsoftJson(x =>
                {
                    x.SerializerSettings.Converters.Add(new NotificationCreationConverter());
                    x.SerializerSettings.Converters.Add(new StringEnumConverter(typeof(CamelCaseNamingStrategy)));
                    x.SerializerSettings.Converters.Add(new PolicyCreationConverter());
                    x.SerializerSettings.Converters.Add(new DiscountCreationConverter());

                }).AddJsonOptions(x => x.JsonSerializerOptions.IgnoreNullValues = true);

            services.AddSignalR(hubOptions =>
            {
                hubOptions.EnableDetailedErrors = true;
                hubOptions.KeepAliveInterval = TimeSpan.FromMilliseconds(500);
                hubOptions.AddFilter<ExceptionHandlingFilter>();
            }).AddNewtonsoftJsonProtocol(x => x.PayloadSerializerSettings.Converters
                .Add(new StringEnumConverter(typeof(CamelCaseNamingStrategy))));
          
            services.AddSwaggerGenNewtonsoftSupport();
            services.AddSwaggerGen(c =>
            {
                c.SchemaFilter<SwaggerExcludeFilter>();
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

            services.AddTransient<AppInitializer>();

            services.Configure<AppInitializationSettings>(Configuration.GetSection(AppInitializationSettings.Section));

            services.AddSingleton<IUseCaseRunner, UseCaseRunner>();

            services.Configure<UseCasesSettings>(Configuration.GetSection(UseCasesSettings.Section));

            services.AddAutoMapper(
                typeof(DomainToDtoProfile),
                typeof(DtoToDomainProfile),
                typeof(DtoToResponseMappingProfile),
                typeof(RequestToDtoMappingProfile));


            switch (DbMode)
            {
                case DbMode.EfCore:
                    services.AddEfCoreDb(ConnectionString);
                    break;
                case DbMode.InMemory:
                    services.AddInMemoryDb();
                    break;
                case DbMode.Mixed:
                    services.AddMixedDb(ConnectionString);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            services.AddTokenAuthentication(Configuration);

            services
                .AddStoresService()
                .AddUsersService()
                .AddPurchasesService(Configuration)
                .AddProductsService();
            
            services.AddSingleton<INotificationPublisher, NotificationPublisher>();
            services.AddSingleton<IConnectionContainer, ConnectionContainer>();

            services.AddTransient<IUseCase, StoreFounderActionsUseCase>();
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

            app.UseSerilogRequestLogging();
            app.UseMiddleware<WebSocketsMiddleware>();
            app.UseMiddleware<ExceptionsMiddleware>();

            app.UseAuthentication();
            app.UseRouting();
            app.UseAuthorization();
            //app.UseHttpsRedirection();


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
