using System.Diagnostics.Metrics;
// ReSharper disable ArrangeObjectCreationWhenTypeNotEvident

namespace OpenTelemetryDemo.Infrastructure.Instrumentation.Metrics;

public class TrafficMetrics
{
    public const string MeterName = $"{TelemetryDefaults.RootName}.Traffic";

    readonly Dictionary<string, int> _queueDepthGauges = new();
    readonly Meter _meter;
    const string MetricPrefix = $"{TelemetryDefaults.RootMetricName}.traffic";

    public TrafficMetrics(IMeterFactory meterFactory)
    {
        _meter = meterFactory.Create(MeterName, tags: [
            TelemetryDefaults.Tags.MachineName
        ]);
    }

    public void SetQueueDepth(string trafficLightName, int size)
    {
        var alreadyExists = _queueDepthGauges.ContainsKey(trafficLightName);
        _queueDepthGauges[trafficLightName] = size;

        if (!alreadyExists)
        {
            _meter.CreateObservableGauge($"{MetricPrefix}.queue_depth", () => new Measurement<int>(
                _queueDepthGauges[trafficLightName],
                tags: [
                    new("traffic_light", trafficLightName)
                ]
            ));
        }
    }
}
