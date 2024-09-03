using System.Diagnostics.Metrics;
using OpenTelemetryDemo.Domain.Abstractions;
// ReSharper disable ArrangeObjectCreationWhenTypeNotEvident

namespace OpenTelemetryDemo.Infrastructure.Instrumentation.Metrics;

public class MessagingMetrics
{
    public const string MeterName = $"{TelemetryDefaults.RootName}.Messaging";

    const string MetricPrefix = $"{TelemetryDefaults.RootMetricName}.messaging";

    readonly Counter<int> _commandsDispatched;
    readonly Counter<int> _eventsDispatched;

    public MessagingMetrics(IMeterFactory meterFactory)
    {
        var meter = meterFactory.Create(MeterName, tags: [
            TelemetryDefaults.Tags.MachineName
        ]);
        _commandsDispatched = meter.CreateCounter<int>($"{MetricPrefix}.commands_dispatched");
        _eventsDispatched = meter.CreateCounter<int>($"{MetricPrefix}.events_dispatched");
    }

    public void CommandDispatched<T>() where T : ICommand => _commandsDispatched.Add(1, tags: [
        new(TelemetryDefaults.TagNames.CommandName, typeof(T).Name)
    ]);

    public void EventDispatched<T>() where T : IEvent => _eventsDispatched.Add(1, tags: [
        new(TelemetryDefaults.TagNames.EventName, typeof(T).Name)
    ]);
}
