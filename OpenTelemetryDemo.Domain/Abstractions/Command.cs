using MediatR;

namespace OpenTelemetryDemo.Domain.Abstractions;

public interface ICommand : IRequest
{
    public Guid Id { get; set; }
    public Guid CausationId { get; set; }
    public string RaisedBy { get; set; }
    public DateTime RaisedAt { get; set; }
}

public abstract class Command : ICommand
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid CausationId { get; set; }
    public string RaisedBy { get; set; } = null!;
    public DateTime RaisedAt { get; set; } = DateTime.UtcNow;
}
