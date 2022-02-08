# Property Logging

An extension of `Microsoft.Extensions.Logging` to format logs using configurable named properties.

### Features

* Harness the power of configuration and/or options to specify what log information is handled by a custom logger.

## Installation

Add the NuGet package to your project:

    $ dotnet add package Logging.Properties

## Usage

Though .NET lets you structure logs in a variety of ways, it is common for different pieces of logged information to be handled similarly.

Property logging lets you implement loggers that write log entries as collections of named values:

```c#
class MyPropertyLogger : PropertyLoggerProvider
{
    public MyPropertyLogger(IEnumerable<ILogPropertyMapper<MyPropertyLogger>> mappers) : base(mappers)
    {
    }

    protected override void Log(IEnumerable<KeyValuePair<string, object>> properties)
    {
        Console.WriteLine("Logged properties: {0}", string.Join(" ", properties));
    }
}
```

The mapping of log information to properties can be configured with `Microsoft.Extensions.Logging.ILoggingBuilder`:

```c#
services.AddLogging(
    builder =>
    {
        // Add the standard property mappers (see below).
        builder.AddProperties<MyPropertyLogger>();

        // Configure "lvl" and "msg" as properties.
        builder.Services.Configure<EntryPropertyOptions<MyPropertyLogger>>(
            opts =>
            {
                opts.Mappings[EntryField.LogLevel] = "lvl";
                opts.Mappings[EntryField.Message] = "msg";
            });

        // Configure stack traces from nested exceptions.
        builder.Services.Configure<ExceptionPropertyOptions<MyPropertyLogger>>(
            opts =>
            {
                opts.Mappings[ExceptionField.StackTrace] = "ex_stack";
                opts.IsRecursive = true;
            });

        // Configure properties from state (includes message template values).
        builder.Services.Configure<StatePropertyOptions<MyPropertyLogger>>(
            opts =>
            {
                opts.Categories["*"] = new()
                {
                    IncludeOthers = true,
                    IncludeScopes = true,
                };
            });

        // Alternatively, load provider options from configuration.
        builder.AddConfiguration(config.GetSection("Logging"));

        // Finally, add your logger to the logger factory.
        builder.Services.AddSingleton<ILoggerProvider, MyPropertyLogger>();
    });
```

### Standard mappers

| Options Model | Configuration Section | Description |
| -- | -- | -- |
| [EntryPropertyOptions](Logging.Properties/Mapping/EntryPropertyOptions.cs) | `Properties:Entry` | Scalar mappings from the top-level log entry. |
| [EventIdPropertyOptions](Logging.Properties/Mapping/EventIdPropertyOptions.cs) | `Properties:EventId` | Mappings from the logged event ID (`Id` and `Name`). |
| [ExceptionPropertyOptions](Logging.Properties/Mapping/ExceptionPropertyOptions.cs) | `Properties:Exception` | Mappings from logged exceptions, including inner exceptions. |
| [EnvironmentPropertyOptions](Logging.Properties/Mapping/EnvironmentPropertyOptions.cs) | `Properties:Environment` | Common mappings from `System.Environment`. |
| [TimestampPropertyOptions](Logging.Properties/Mapping/TimestampPropertyOptions.cs) | `Properties:Timestamp` | Mapping from `DateTimeOffset.Now`. |
| [StaticPropertyOptions](Logging.Properties/Mapping/StaticPropertyOptions.cs) | `Properties:Static` | Injection of static custom properties. |
| [StatePropertyOptions](Logging.Properties/Mapping/StatePropertyOptions.cs) | `Properties:State` | Mappings from entry and scope state by category, including values logged with a message template. |

Register your own implementation of `ILogPropertyMapper<TProvider>` to map other kinds of data!

### Example configuration

An example of ECS (Elasic Common Schema) mappings in `appsettings.json`:

```json
{
  "Logging": {
    "MyApp.MyPropertyLogger": {
      "LogLevel": {
        "Default": "Debug"
      },
      "Properties": {
        "Entry": {
          "Mappings": {
            "LogLevel": "log.level",
            "Category": "log.logger",
            "Message": "message"
          }
        },
        "EventId": {
          "Mappings": {
            "Id": "event.code",
            "Name": "event.action"
          }
        },
        "Exception": {
          "Mappings": {
            "Type": "error.type",
            "Message": "error.message",
            "StackTrace": "error.stack_trace"
          },
          "IsRecursive": true
        },
        "State": {
          "Categories": {
            "System.Net.Http.HttpClient": {
              "Mappings": {
                "HttpMethod": "http.request.method",
                "Uri": "url.original",
                "StatusCode": "http.response.status_code"
              }
            }
          }
        },
        "Timestamp": {
          "Mapping": "@timestamp"
        },
        "Environment": {
          "Mappings": {
            "MachineName": "host.hostname",
            "ProcessId": "process.pid"
          }
        },
        "Static": {
          "Values": {
            "ecs.version": "1.12.1"
          }
        }
      }
    }
  }
}
```
