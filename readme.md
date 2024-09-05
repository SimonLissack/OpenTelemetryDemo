# OpenTelemetry demo

A small event-based system which models a traffic light junction

# Setup

F5 `OpenTelemetryDemo.Cli`. `launchsettings.json` contains the details for sending metrics to the otel-collctor.

To run the metrics server: `docker compose -f .metrics/docker-compose.yml up -d`
