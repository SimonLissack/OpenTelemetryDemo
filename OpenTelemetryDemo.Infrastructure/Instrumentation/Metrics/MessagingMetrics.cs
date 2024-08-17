using System.Diagnostics.Metrics;
using OpenTelemetryDemo.Domain.Abstractions;

namespace OpenTelemetryDemo.Infrastructure.Instrumentation.Metrics;

public class MessagingMetrics
{
    readonly Counter<int> _commandsDispatched;
    readonly Counter<int> _eventsDispatched;

    public MessagingMetrics(IMeterFactory meterFactory)
    {
        var meter = meterFactory.Create("OpenTelemetryDemo.Messaging", tags: [
            new(Tags.MachineName, Environment.MachineName)
        ]);
        _commandsDispatched = meter.CreateCounter<int>("otel_demo.messaging.commands_dispatched");
        _eventsDispatched = meter.CreateCounter<int>("otel_demo.messaging.events_dispatched");
    }

    public void CommandDispatched<T>() where T : ICommand => _commandsDispatched.Add(1, tags: [
        new(Tags.CommandName, typeof(T).Name)
    ]);

    public void EventDispatched<T>() where T : IEvent => _eventsDispatched.Add(1, tags: [
        new(Tags.EventName, typeof(T).Name)
    ]);
}

file static class Tags
{
    public const string MachineName = "machine_name";

    public const string CausationId = "causation_id";
    public const string EventName = "event_type";
    public const string CommandName = "event_type";
}
