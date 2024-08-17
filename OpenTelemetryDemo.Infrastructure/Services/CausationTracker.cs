using OpenTelemetryDemo.Domain.Abstractions;

namespace OpenTelemetryDemo.Infrastructure.Services;

public class CausationTracker
{
    readonly Guid _correlationId;
    Guid _currentCausationId;

    public CausationTracker()
    {
        _correlationId = Guid.NewGuid();
        _currentCausationId = _correlationId;
    }

    public void ApplyCausation(IMessage message)
    {
        message.CorrelationId = _correlationId;
        message.CausationId = _currentCausationId;
        _currentCausationId = message.Id;
    }
}
