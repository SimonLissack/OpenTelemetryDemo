using OpenTelemetryDemo.Domain.Abstractions;
using OpenTelemetryDemo.Domain.TrafficLights.Models;

namespace OpenTelemetryDemo.Domain.TrafficLights.Commands;

public class RequestTransition : TrafficLightCommand
{
    public TrafficLightState RequestedLightState { get; set; }
}
