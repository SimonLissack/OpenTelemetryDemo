using OpenTelemetryDemo.Domain.Abstractions;

namespace OpenTelemetryDemo.Infrastructure.Services;

public class CausationTracker
{
    Guid _currentCausationId;
    public Guid CorrelationId { get; }

    public CausationTracker()
    {
        CorrelationId = Guid.NewGuid();
        _currentCausationId = CorrelationId;
    }

    public void ApplyCausation(IMessage message)
    {
        message.CorrelationId = CorrelationId;
        message.CausationId = _currentCausationId;
        _currentCausationId = message.Id;
    }
}
