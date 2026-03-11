using TodoApi.Application.Events;
using TodoApi.Domain.Events;
using TodoApi.Models;
using TodoApi.Services;

namespace TodoApi.Application.Todos.Commands;

public class TodoCommandHandler : ITodoCommandHandler
{
    private readonly ITodoService _todoService;
    private readonly IEventPublisher _eventPublisher;

    public TodoCommandHandler(ITodoService todoService, IEventPublisher eventPublisher)
    {
        _todoService = todoService;
        _eventPublisher = eventPublisher;
    }

    public TodoItem Create(CreateTodoCommand command)
    {
        var created = _todoService.Add(command.Title);
        _eventPublisher.PublishAsync(new TodoCreatedEvent(created.Id, created.Title, DateTime.UtcNow)).GetAwaiter().GetResult();
        return created;
    }

    public TodoItem? Update(UpdateTodoCommand command)
    {
        var updated = _todoService.Update(command.Id, command.Title, command.IsDone);
        if (updated is not null)
        {
            _eventPublisher.PublishAsync(
                new TodoUpdatedEvent(updated.Id, updated.Title, updated.IsDone, DateTime.UtcNow))
                .GetAwaiter()
                .GetResult();
        }

        return updated;
    }

    public TodoItem? Complete(CompleteTodoCommand command)
    {
        var existing = _todoService.GetById(command.Id);
        if (existing is null)
        {
            return null;
        }

        var completed = _todoService.Update(command.Id, existing.Title, true);
        if (completed is not null)
        {
            _eventPublisher.PublishAsync(
                new TodoCompletedEvent(completed.Id, completed.Title, DateTime.UtcNow))
                .GetAwaiter()
                .GetResult();
        }

        return completed;
    }

    public bool Delete(DeleteTodoCommand command)
    {
        var deleted = _todoService.Delete(command.Id);
        if (deleted)
        {
            _eventPublisher.PublishAsync(new TodoDeletedEvent(command.Id, DateTime.UtcNow)).GetAwaiter().GetResult();
        }

        return deleted;
    }
}
