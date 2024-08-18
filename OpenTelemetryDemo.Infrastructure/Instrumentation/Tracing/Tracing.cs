using System.Diagnostics;
using OpenTelemetryDemo.Infrastructure.Services;

namespace OpenTelemetryDemo.Infrastructure.Instrumentation.Tracing;

public class Tracing(CausationTracker causationTracker)
{
    static readonly ActivitySource ActivitySource = new(TelemetryDefaults.RootName);

    public Activity? StartActivity(string name)
    {
        var activity = ActivitySource.StartActivity(name);
        activity?.SetTag(TelemetryDefaults.TagNames.CorrelationId, causationTracker.CorrelationId);

        return activity;
    }
}
