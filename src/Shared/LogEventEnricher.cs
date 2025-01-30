using Serilog.Core;
using Serilog.Events;

namespace Shared;

/// <summary>
/// Enriches SeriLog
/// </summary>
public class LogEventEnricher : ILogEventEnricher
{
    public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
    {
        GenerateShortSourceName(logEvent, propertyFactory);

        GenerateShortLogLevel(logEvent, propertyFactory);
    }

    static void GenerateShortLogLevel(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
    {
        var levelString = logEvent.Level switch
        {
            LogEventLevel.Information => "Info",
            LogEventLevel.Warning => "Warn",
            LogEventLevel.Error => "Err",
            LogEventLevel.Debug => "Dbg",
            LogEventLevel.Fatal => "Fatl",
            LogEventLevel.Verbose => "Verb",
            _ => logEvent.Level.ToString()
        };
        logEvent.AddPropertyIfAbsent(propertyFactory.CreateProperty("lvl", levelString));
    }

    static void GenerateShortSourceName(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
    {
        if (logEvent.Properties.TryGetValue("SourceContext", out var sourceContextProperty))
        {
            var fullName = sourceContextProperty.ToString().Trim('"');
            var shortName = fullName.Split('.').Last();
            logEvent.AddPropertyIfAbsent(propertyFactory.CreateProperty(
                "ShortSourceContext", shortName));
        }
    }
}