﻿using System.Diagnostics.Metrics;
using OpenTelemetryDemo.Domain.TrafficLights.Models;

// ReSharper disable ArrangeObjectCreationWhenTypeNotEvident

namespace OpenTelemetryDemo.Infrastructure.Instrumentation.Metrics;

public class TrafficMetrics
{
    readonly Histogram<int> _queuedTraffic;
    const string MetricPrefix = $"{MetricsDefaults.RootMetricName}.traffic";

    public TrafficMetrics(IMeterFactory meterFactory)
    {
        var meter = meterFactory.Create($"{MetricsDefaults.RootName}.Traffic", tags: [
            MetricsDefaults.Tags.MachineName
        ]);

        _queuedTraffic = meter.CreateHistogram<int>($"{MetricPrefix}.queue_depth");
    }

    public void UpdateQueuedTraffic(string trafficLightName, int queue, TrafficLightState trafficLightState)
    {
        _queuedTraffic.Record(queue, tags: [
            new("traffic_light", trafficLightName),
            new("light_state", trafficLightState.ToString())
        ]);
    }
}