using TodoApi.Domain.Events;

namespace TodoApi.Application.Events;

public interface IEventPublisher
{
    Task PublishAsync(IDomainEvent domainEvent, CancellationToken cancellationToken = default);
}
