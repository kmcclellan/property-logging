# Property Logging

An extension of `Microsoft.Extensions.Logging` to format logs using configurable named properties.

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
        // Add the default property mappers (see XML documentation).
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
