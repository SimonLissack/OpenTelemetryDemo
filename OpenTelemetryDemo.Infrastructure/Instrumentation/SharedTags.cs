namespace OpenTelemetryDemo.Infrastructure.Instrumentation;

public static class MetricsDefaults
{
    public const string RootName = "OpenTelemetryDemo";
    public const string RootMetricName = "otel_demo";

    public static class Tags
    {
        public static readonly KeyValuePair<string, object?> MachineName = new ("machine_name", Environment.MachineName);

    }
}
