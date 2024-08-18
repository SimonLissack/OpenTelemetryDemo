using System.Diagnostics;

namespace OpenTelemetryDemo.Infrastructure.Instrumentation.Tracing;

public static class TracingExtensions
{
    public static void TagObject<T>(this Activity? activity, string tagName) => activity?.SetTag(tagName, typeof(T).Name);
}
