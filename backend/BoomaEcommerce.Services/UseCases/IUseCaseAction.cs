using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;

namespace BoomaEcommerce.Services.UseCases
{
    public interface IUseCaseAction
    {
        public IUseCaseAction NextUseCaseAction { get; set; }
        public Task NextAction(Dictionary<string,object> dict = null, ClaimsPrincipal claims = null);
    }

    public abstract class UseCaseAction : IUseCaseAction
    {
        [JsonIgnore]
        public IUseCaseAction NextUseCaseAction { get; set; }

        [JsonIgnore]
        public IServiceProvider Sp { get; set; }
        
        [JsonRequired]
        public string Label { get; set; }


        private readonly IHttpContextAccessor _accessor;

        protected UseCaseAction(IUseCaseAction next, IServiceProvider serviceProvider, IHttpContextAccessor accessor)
        {
            Sp = serviceProvider;
            _accessor = accessor;
            NextUseCaseAction = next;
        }

        protected UseCaseAction(IServiceProvider serviceProvider, IHttpContextAccessor accessor)
        {
            Sp = serviceProvider;
            _accessor = accessor;
        }

        protected UseCaseAction()
        {
        }

        protected async Task Next(Dictionary<string,object> dict = null, ClaimsPrincipal claims = null)
        {
            if (NextUseCaseAction != null)
            {

                _accessor.HttpContext = claims != null
                    ? new DefaultHttpContext { User = claims }
                    : null;

                await NextUseCaseAction.NextAction(dict, claims);

                _accessor.HttpContext = null;
            }
        }

        public abstract Task NextAction(Dictionary<string,object> dict = null, ClaimsPrincipal claims = null);
    }
}
