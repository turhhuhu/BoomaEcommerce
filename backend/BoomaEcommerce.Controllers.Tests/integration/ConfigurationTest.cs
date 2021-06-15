using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BoomaEcommerce.Api.Config;
using BoomaEcommerce.Services.Settings;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Xunit;

namespace BoomaEcommerce.Controllers.Tests.integration
{

    public class ConfigurationTest
    {

        [Fact]
        public void ConfigTest_ShouldReadFileSuccessfullyWithCorrectValues()
        {
            var configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: true)
                .Build();

            var jwtSettings = configuration.GetSection("Jwt").Get<JwtSettings>();
            var dbMode = configuration.GetSection("DbMode").Get<DbMode>();
            var useStubExternalSystems = configuration.GetSection("UseStubExternalSystems").Get<bool>();
            var initSettings = configuration.GetSection("AppInitialization").Get<AppInitializationSettings>();
            var connectionString = configuration.GetConnectionString("DefaultConnectionString");

            useStubExternalSystems.Should().BeFalse();

            connectionString.Should().NotBeNullOrWhiteSpace();

            initSettings.AdminPassword.Should().NotBeNullOrWhiteSpace();
            initSettings.AdminUserName.Should().NotBeNullOrWhiteSpace();
            initSettings.SeedDummyData.Should().BeFalse();

            dbMode.Should().Be(DbMode.EfCore);

            jwtSettings.Should().NotBeNull();
            jwtSettings.RefreshTokenExpirationMonthsAmount.Should().NotBe(default);
            jwtSettings.Secret.Should().NotBe(default);
            jwtSettings.TokenLifeTime.Should().NotBe(default);

        }
    }
}
