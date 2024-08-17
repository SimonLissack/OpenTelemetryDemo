using OpenTelemetryDemo.Domain.Abstractions;

namespace OpenTelemetryDemo.Domain.TrafficLights.Commands;

public class TrafficLightCommand : Command
{
    public string TrafficLightName { get; set; } = null!;
}
