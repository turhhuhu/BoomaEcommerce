using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace BoomaEcommerce.Services.UseCases
{
    public interface IUseCaseAction
    {
        public IUseCaseAction NextUseCaseAction { get; set; }
        public Task NextAction(ClaimsPrincipal claims = null);
    }

    public abstract class UseCaseAction : IUseCaseAction
    {
        [JsonIgnore]
        public IUseCaseAction NextUseCaseAction { get; set; }

        [JsonIgnore]
        public IServiceProvider Sp { get; set; }

        protected UseCaseAction(IUseCaseAction next, IServiceProvider serviceProvider)
        {
            NextUseCaseAction = next;
            Sp = serviceProvider;
        }

        protected UseCaseAction()
        {
            
        }

        protected Task Next(ClaimsPrincipal claims = null)
        {
            return NextUseCaseAction != null 
                ? NextUseCaseAction.NextAction(claims) 
                : Task.CompletedTask;
        }

        public virtual Task NextAction(ClaimsPrincipal claims = null)
        {
            return Task.CompletedTask;
        }
    }
}
