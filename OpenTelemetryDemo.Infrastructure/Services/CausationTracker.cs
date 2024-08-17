using OpenTelemetryDemo.Domain.Abstractions;

namespace OpenTelemetryDemo.Infrastructure.Services;

public class CausationTracker
{
    Guid _currentCausationId = Guid.NewGuid();

    public void ApplyCausation(IMessage message)
    {
        message.CausationId = _currentCausationId;
        _currentCausationId = message.Id;
    }
}
