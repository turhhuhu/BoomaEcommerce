using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BoomaEcommerce.Services.Settings;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace BoomaEcommerce.Services.UseCases
{
    public interface IUseCaseRunner
    {
        public Task RunAsync();
    }
    public class UseCaseSettings
    {
        public string FilePath { get; set; }
    }


    public class UseCase
    {
        public IUseCaseAction[] Actions { get; set; }
    }

    public class UseCaseRunner : IUseCaseRunner
    {
        private readonly IServiceProvider _sp;
        private readonly JwtSettings _jwtSettings;
        private readonly UseCaseSettings _useCaseSettings;

        public UseCaseRunner(IServiceProvider sp, JwtSettings jwtSettings, UseCaseSettings useCaseSettings)
        {
            _sp = sp;
            _jwtSettings = jwtSettings;
            _useCaseSettings = useCaseSettings;
        }

        public Task RunAsync()
        {
            throw new NotImplementedException();
        }

        public void ReadUseCases()
        {
            var jsonString = File.ReadAllText(_useCaseSettings.FilePath);
            var useCases = JsonConvert.DeserializeObject(jsonString, new JsonSerializerSettings
            {
                Converters = new List<JsonConverter>
                {

                }
            });
        }
    }
}
