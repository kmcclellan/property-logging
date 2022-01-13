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
    public MyPropertyLogger(ILogPropertyMapper<MyPropertyLogger> mapper) : base(mapper)
    {
    }

    protected override Log(IEnumerable<KeyValuePair<string, object>> properties)
    {
        foreach (var (key, value) in properties)
        {
            Console.WriteLine("Logged property '{0}': {1}", key, value);
        }
    }
}
```

By default, properties will contain any logged state or scope implementing `IEnumerable<KeyValuePair<string, object>>`.

This includes values logged with a message template:

```c#
// Adds [RoomNumber, 1234] to the properties.
using (logger.BeginScope("Taking attendance for {RoomNumber}...", 1234))
{
    // Adds [Name, Roger] to the properties.
    logger.LogInformation("Hello, {Name}!", "Roger")`);
}
```

The mapping of additional log information to properties is configurable with `Microsoft.Extensions.Logging.ILoggingBuilder`:

```c#
services.AddLogging(
    builder =>
    {
        builder.AddProperties<MyPropertyLogger>()
            .FromEntry(level: "lvl", message: "msg")
            .FromException("ex_stack", ex => ex.StackTrace)
            .FromValue("timestamp", () => DateTimeOffset.Now);
    });
```

You can use the builder services to register your logger with the factory:

```c#
builder.Services.AddSingleton<ILoggerProvider, MyPropertyLogger>();
```
