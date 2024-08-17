namespace OpenTelemetryDemo.Domain.TrafficLights.Commands;

public class AddArrivingTraffic : TrafficLightCommand
{
    public int TrafficArrived { get; set; } = 0;
}
