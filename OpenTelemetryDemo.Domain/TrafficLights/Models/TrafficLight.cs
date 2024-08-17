namespace OpenTelemetryDemo.Domain.TrafficLights.Models;

public class TrafficLight
{
    public string Name { get; set; } = null!;
    public TrafficLightState LightState { get; set; }
    public int QueuedTraffic { get; set; } = 0;
    public DateTime LastTransition { get; set; } = DateTime.UtcNow;
}

public enum TrafficLightState
{
    Red,
    Green
}
