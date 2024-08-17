namespace OpenTelemetryDemo.Domain.TrafficLights.Models;

public class TrafficLightAlreadyExistsException : Exception
{
    public TrafficLightAlreadyExistsException(string name) : base($"Traffic light {name} already exists")
    {
    }
}
