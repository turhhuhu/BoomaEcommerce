using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BoomaEcommerce.Services.Settings;
using BoomaEcommerce.Services.UseCases;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Serilog;

namespace BoomaEcommerce.Api
{
    public interface IUseCaseRunner
    {
        public void RunAsync();
    }
    public class UseCasesSettings
    {
        public const string Section = "UseCases";
        public string FilePath { get; set; }
    }


    public class UseCase
    {
        public IUseCaseAction[] Actions { get; set; }
    }
    public class UseCases
    {
        [JsonProperty("UseCases")]
        public UseCase[] Value { get; set; }
    }

    public class UseCaseRunner : IUseCaseRunner
    {
        private readonly IServiceProvider _sp;
        private readonly JwtSettings _jwtSettings;
        private readonly UseCasesSettings _useCasesSettings;
        private readonly IHttpContextAccessor _accesor;
        private readonly ILogger<UseCaseRunner> _logger;

        public UseCaseRunner(IServiceProvider sp, IOptions<JwtSettings> jwtSettings, IOptions<UseCasesSettings> useCaseSettings, IHttpContextAccessor accesor, ILogger<UseCaseRunner> logger)
        {
            _sp = sp;
            _accesor = accesor;
            _logger = logger;
            _jwtSettings = jwtSettings.Value;
            _useCasesSettings = useCaseSettings.Value;
        }

        public void RunAsync()
        {
            try
            {
                var useCases = ReadUseCases();
                useCases.Value.ToList().ForEach(useCase =>
                {
                    useCase.Actions.Aggregate((acc, curr) => acc.NextUseCaseAction = curr);
                });

                var useCaseAggregated =
                    useCases.Value
                        .Select(useCase => useCase.Actions.FirstOrDefault())
                        .Where(u => u != null)
                        .ToList();

                useCaseAggregated.ForEach(async u =>
                {
                    for (int i = 0; i < u.AmountToRun; i++)
                    {
                        await u.NextAction();
                    }
                });
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Failed to run use cases");
            }

        }
        
        public UseCases ReadUseCases()
        {
            var jsonString = File.ReadAllText(_useCasesSettings.FilePath);
            var useCases = JsonConvert.DeserializeObject<UseCases>(jsonString,
                new JsonSerializerSettings
                {
                    Converters = new List<JsonConverter>
                    {
                        new UseCaseActionCreationConverter(_jwtSettings, _sp, _accesor)
                    }
                });
            return useCases;
        }
    }
}
