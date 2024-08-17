namespace OpenTelemetryDemo.Domain.TrafficLights.Models;

public class TrafficLightDoesNotExistException : Exception
{
    public TrafficLightDoesNotExistException(string name) : base($"Traffic light {name} already exists")
    {
    }
}
