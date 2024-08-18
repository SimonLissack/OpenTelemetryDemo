namespace OpenTelemetryDemo.Infrastructure.Instrumentation;

public static class TelemetryDefaults
{
    public const string RootName = "OpenTelemetryDemo";
    public const string RootMetricName = "otel_demo";

    public static class Tags
    {
        public static readonly KeyValuePair<string, object?> MachineName = new (TagNames.MachineName, Environment.MachineName);
    }

    public static class TagNames
    {
        public const string MachineName = "machine_name";
        public const string CorrelationId = "correlation_id";
        public const string CommandName = "command_name";
        public const string EventName = "event_name";
    }
}
