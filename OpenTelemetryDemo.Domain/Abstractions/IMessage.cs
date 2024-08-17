namespace OpenTelemetryDemo.Domain.Abstractions;

public interface IMessage
{
    public Guid CausationId { get; set; }
    public Guid Id { get; set; }
}
