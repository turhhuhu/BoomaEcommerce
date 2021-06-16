
# BoomaEcommerce 

## Init File

The Init file is a JSON file that has a list of Actions that will run one after another before the system goes up.  
to add a new action to the init file, you need to check its Use case Action class and find what arguments it should get.  
each use case has a Label parameter, which represent the 'returned value' to pass through to the next action.



### For Example


```json
 {
        "Type": "CreateStoreAction",
        "UserLabel": "Benny User",
        "Label" : "Rami Levi",
        "StoreToCreate": 
        {
           "StoreName": "Rami Levi",
           "Description": "Blah"
        }
  }
```

create store action need to get User label which represent the user that want to open a store, Label which will direct any future action to this store and some new store details. 



### Configuration file

The configuration file is "appsettings.{EnvironmentVariable}.json" and is applied depending on the environment variable set in the current system.
In every environment the settings are set to fit the needs of the environement.

### Jwt section:
The jwt secition has fields that are related to jwt token authenticatioמ:
  Secret - The secret used to generate the tokens.
  TokenLifeTime - The token life time.
  RefreshTokenExpirationMonthsAmount - the refresh token life time.
  
## AppInitialization:
  SeedDummyData - Boolean field decided weither some dummy data is seeded contains a store and products for admin.
  AdminUserName - The username of the admin.
  AdminPassword - The password of the admin
  
## UseCases:
  RunUseCases - Boolean field deicdes weither the use cases are run.
  FilePath - The path of the use case file (init file).
  
## DbMode: 
 EfCore\InMemory decides weither the server is run with EntityFramework ORM or in memory.

## UseStubExternalSystems:
 Boolean field decides weither the server is run with stub external systems.
 
## ConnectionStrings:
  Connection strings of the system.
  DefaultConnectionString - The connection string of the Sql server database.
  
## Serilog
  Log related configuration
  
example of a config file:
```
{
  "Jwt": {
    "Secret": "",
    "TokenLifeTime": "00:00:00",
    "RefreshTokenExpirationMonthsAmount": 1
  },
  "AppInitialization": {
    "SeedDummyData": false,
    "AdminUserName": "Admin",
    "AdminPassword": "Admin",
  },
  "UseCases": {
    "RunUseCases": false,
    "FilePath": "./usecases.json"
  },
  "DbMode": "EfCore",
  "UseStubExternalSystems": false,
  "ConnectionStrings": {
    "DefaultConnectionString": ""
  },
  "Serilog": {
    "Using": [],
    "MinimumLevel": {
      "Default": "Information",
      "Override": {
        "Microsoft": "Warning",
        "System": "Warning",
        "Microsoft.Hosting": "Information"
      }
    },
    "Enrich": [ "FromLogContext", "WithMachineName", "WithProcessId" ],
    "WriteTo": [
      {
        "Name": "Console",
        "Args": {
          "theme": "Serilog.Sinks.SystemConsole.Themes.AnsiConsoleTheme::Code, Serilog.Sinks.Console",
          "outputTemplate": "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj} <s:{SourceContext}>{NewLine}{Exception}"
        }
      },
      {
        "Name": "File",
        "Args": {
          "path": ".\\Logs\\log-.txt",
          "outputTemplate": "{Timestamp:G} {Message}{NewLine:1}{Exception:1}",
          "rollingInterval": "Day"
        }
      }
    ]

  },
  "AllowedHosts": "*"
}
```

### Test configuration file
The file is a similar config file "appsettings.Test.json" that decides weither the the server runs tests on database\mocked and weither the server
runs with mocked\real external systems.


workshop in software engineering project
Matan Hazan 315198796
Omer Kempner 322217472
Ori Kintzlinger 318929213
Arye Shapiro 313578379 
Benny Skidanov 322572926
