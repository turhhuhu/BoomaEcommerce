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
                "LoginAction" => new LoginUseCaseAction {JwtSettings = _jwtSettings, Sp = _serviceProvider},
                "CreateStoreAction" => new CreateStoreUseCaseAction {Sp = _serviceProvider, Accessor = _httpContextAccessor},
                _ => throw new ArgumentOutOfRangeException()
            };
        }
    }
}
