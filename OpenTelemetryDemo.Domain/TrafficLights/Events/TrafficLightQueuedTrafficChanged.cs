namespace OpenTelemetryDemo.Domain.TrafficLights.Events;

public class TrafficLightQueuedTrafficChanged : TrafficLightEvent
{
    public int NewTrafficCount { get; set; } = 0;
}
