namespace OpenTelemetryDemo.Domain.TrafficLights.Commands;

public class RemoveLeavingTraffic : TrafficLightCommand
{
    public int TrafficLeaving { get; set; } = 0;
}
