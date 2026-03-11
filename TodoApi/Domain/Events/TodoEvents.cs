namespace TodoApi.Domain.Events;

public interface IDomainEvent
{
    DateTime OccurredAtUtc { get; }
}

public record TodoCreatedEvent(int TodoId, string Title, DateTime OccurredAtUtc) : IDomainEvent;
public record TodoUpdatedEvent(int TodoId, string Title, bool IsDone, DateTime OccurredAtUtc) : IDomainEvent;
public record TodoCompletedEvent(int TodoId, string Title, DateTime OccurredAtUtc) : IDomainEvent;
public record TodoDeletedEvent(int TodoId, DateTime OccurredAtUtc) : IDomainEvent;
