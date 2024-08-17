using MediatR;

namespace OpenTelemetryDemo.Domain.Abstractions;

public interface ICommand : IMessage, IRequest
{
    public string RaisedBy { get; set; }
    public DateTime RaisedAt { get; set; }
}
