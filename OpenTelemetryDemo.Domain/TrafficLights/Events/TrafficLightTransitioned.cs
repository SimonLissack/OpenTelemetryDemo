using OpenTelemetryDemo.Domain.TrafficLights .Models;

namespace OpenTelemetryDemo.Domain.TrafficLights.Events;

public class TrafficLightTransitioned : TrafficLightEvent
{
    public TrafficLightState TrafficLightState { get; set; }
}
