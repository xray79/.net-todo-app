using TodoApi.Application.Events;
using TodoApi.Domain.Events;

namespace TodoApi.Infrastructure.Events;

public class LoggingEventPublisher : IEventPublisher
{
    private readonly ILogger<LoggingEventPublisher> _logger;

    public LoggingEventPublisher(ILogger<LoggingEventPublisher> logger)
    {
        _logger = logger;
    }

    public Task PublishAsync(IDomainEvent domainEvent, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Published domain event {EventType} at {OccurredAtUtc}", domainEvent.GetType().Name, domainEvent.OccurredAtUtc);
        return Task.CompletedTask;
    }
}
