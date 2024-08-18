using System.Diagnostics.Metrics;
using OpenTelemetryDemo.Domain.Abstractions;
// ReSharper disable ArrangeObjectCreationWhenTypeNotEvident

namespace OpenTelemetryDemo.Infrastructure.Instrumentation.Metrics;

public class MessagingMetrics
{
    const string MetricPrefix = $"{TelemetryDefaults.RootMetricName}.Messaging";

    readonly Counter<int> _commandsDispatched;
    readonly Counter<int> _eventsDispatched;

    public MessagingMetrics(IMeterFactory meterFactory)
    {
        var meter = meterFactory.Create($"{TelemetryDefaults.RootName}.Messaging", tags: [
            TelemetryDefaults.Tags.MachineName
        ]);
        _commandsDispatched = meter.CreateCounter<int>($"{MetricPrefix}.commands_dispatched");
        _eventsDispatched = meter.CreateCounter<int>($"{MetricPrefix}.events_dispatched");
    }

    public void CommandDispatched<T>() where T : ICommand => _commandsDispatched.Add(1, tags: [
        new("command_name", typeof(T).Name)
    ]);

    public void EventDispatched<T>() where T : IEvent => _eventsDispatched.Add(1, tags: [
        new("event_name", typeof(T).Name)
    ]);
}
