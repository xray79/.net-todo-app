using FluentAssertions;
using Moq;
using TodoApi.Application.Events;
using TodoApi.Application.Todos.Commands;
using TodoApi.Domain.Events;
using TodoApi.Models;
using TodoApi.Services;
using Xunit;

namespace TodoApi.UnitTests;

public class TodoCommandHandlerTests
{
    private readonly Mock<ITodoService> _todoService = new();
    private readonly Mock<IEventPublisher> _eventPublisher = new();
    private readonly List<IDomainEvent> _publishedEvents = [];

    [Fact]
    public void Create_PublishesTodoCreatedEvent()
    {
        var created = new TodoItem(1, "Write tests", false);
        _todoService.Setup(s => s.Add("Write tests")).Returns(created);
        var sut = CreateSut();

        var result = sut.Create(new CreateTodoCommand("Write tests"));

        result.Should().Be(created);
        _publishedEvents.Should().ContainSingle()
            .Which.Should().BeOfType<TodoCreatedEvent>()
            .Which.TodoId.Should().Be(created.Id);
    }

    [Fact]
    public void Update_WhenTodoExists_PublishesTodoUpdatedEvent()
    {
        var updated = new TodoItem(3, "Updated title", true);
        _todoService.Setup(s => s.Update(3, "Updated title", true)).Returns(updated);
        var sut = CreateSut();

        var result = sut.Update(new UpdateTodoCommand(3, "Updated title", true));

        result.Should().Be(updated);
        _publishedEvents.Should().ContainSingle()
            .Which.Should().BeOfType<TodoUpdatedEvent>()
            .Which.TodoId.Should().Be(updated.Id);
    }

    [Fact]
    public void Complete_WhenTodoExists_PublishesTodoCompletedEvent()
    {
        _todoService.Setup(s => s.GetById(7)).Returns(new TodoItem(7, "Complete me", false));
        _todoService.Setup(s => s.Update(7, "Complete me", true)).Returns(new TodoItem(7, "Complete me", true));
        var sut = CreateSut();

        var result = sut.Complete(new CompleteTodoCommand(7));

        result.Should().NotBeNull();
        result!.IsDone.Should().BeTrue();
        _publishedEvents.Should().ContainSingle()
            .Which.Should().BeOfType<TodoCompletedEvent>()
            .Which.TodoId.Should().Be(7);
    }

    [Fact]
    public void Delete_WhenTodoExists_PublishesTodoDeletedEvent()
    {
        _todoService.Setup(s => s.Delete(9)).Returns(true);
        var sut = CreateSut();

        var result = sut.Delete(new DeleteTodoCommand(9));

        result.Should().BeTrue();
        _publishedEvents.Should().ContainSingle()
            .Which.Should().BeOfType<TodoDeletedEvent>()
            .Which.TodoId.Should().Be(9);
    }

    [Fact]
    public void Complete_WhenTodoDoesNotExist_DoesNotPublishEvent()
    {
        _todoService.Setup(s => s.GetById(77)).Returns((TodoItem?)null);
        var sut = CreateSut();

        var result = sut.Complete(new CompleteTodoCommand(77));

        result.Should().BeNull();
        _publishedEvents.Should().BeEmpty();
    }

    private TodoCommandHandler CreateSut()
    {
        _eventPublisher
            .Setup(p => p.PublishAsync(It.IsAny<IDomainEvent>(), It.IsAny<CancellationToken>()))
            .Callback<IDomainEvent, CancellationToken>((evt, _) => _publishedEvents.Add(evt))
            .Returns(Task.CompletedTask);
        return new TodoCommandHandler(_todoService.Object, _eventPublisher.Object);
    }
}
