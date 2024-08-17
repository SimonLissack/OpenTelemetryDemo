using OpenTelemetryDemo.Domain.Abstractions;

namespace OpenTelemetryDemo.Domain.TrafficLights.Events;

public abstract class TrafficLightEvent : Event
{
    public string TrafficLightName { get; set; } = null!;
}
