using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using TodoApi.Application.Todos.Commands;
using TodoApi.Application.Todos.Queries;
using TodoApi.Controllers;
using TodoApi.Models;
using Xunit;

namespace TodoApi.UnitTests;

public class TodosControllerTests
{
    private readonly Mock<ITodoCommandHandler> _commandHandler = new();
    private readonly Mock<ITodoQueryHandler> _queryHandler = new();

    private TodosController CreateController() => new(_commandHandler.Object, _queryHandler.Object);

    [Fact]
    public void GetAll_ShouldReturnTodos()
    {
        var todos = new List<TodoItem>
        {
            new(1, "One", false),
            new(2, "Two", true)
        };

        _queryHandler.Setup(handler => handler.GetAll(It.IsAny<GetTodosQuery>())).Returns(todos);
        var controller = CreateController();

        var result = controller.GetAll();

        var ok = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        ok.Value.Should().BeEquivalentTo(todos);
    }

    [Fact]
    public void GetById_ShouldReturnNotFound_WhenTodoDoesNotExist()
    {
        _queryHandler.Setup(handler => handler.GetById(new GetTodoByIdQuery(99))).Returns((TodoItem?)null);
        var controller = CreateController();

        var result = controller.GetById(99);

        result.Result.Should().BeOfType<NotFoundResult>();
    }

    [Fact]
    public void Create_ShouldReturnCreatedAtAction_WhenRequestIsValid()
    {
        var created = new TodoItem(10, "Write tests", false);
        _commandHandler.Setup(handler => handler.Create(new CreateTodoCommand("Write tests"))).Returns(created);
        var controller = CreateController();

        var result = controller.Create(new CreateTodoRequest { Title = "Write tests" });

        var createdResult = result.Result.Should().BeOfType<CreatedAtActionResult>().Subject;
        createdResult.ActionName.Should().Be(nameof(TodosController.GetById));
        createdResult.Value.Should().BeEquivalentTo(created);
    }

    [Fact]
    public void Update_ShouldReturnUpdatedTodo_WhenTodoExists()
    {
        var updated = new TodoItem(2, "Updated", true);
        _commandHandler
            .Setup(handler => handler.Update(new UpdateTodoCommand(2, "Updated", true)))
            .Returns(updated);
        var controller = CreateController();

        var result = controller.Update(2, new UpdateTodoRequest { Title = "Updated", IsDone = true });

        var ok = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        ok.Value.Should().BeEquivalentTo(updated);
    }

    [Fact]
    public void Update_ShouldReturnNotFound_WhenTodoDoesNotExist()
    {
        _commandHandler
            .Setup(handler => handler.Update(new UpdateTodoCommand(123, "Missing", true)))
            .Returns((TodoItem?)null);
        var controller = CreateController();

        var result = controller.Update(123, new UpdateTodoRequest { Title = "Missing", IsDone = true });

        result.Result.Should().BeOfType<NotFoundResult>();
    }

    [Fact]
    public void Complete_ShouldReturnOk_WhenTodoExists()
    {
        var completed = new TodoItem(3, "Complete me", true);
        _commandHandler
            .Setup(handler => handler.Complete(new CompleteTodoCommand(3)))
            .Returns(completed);
        var controller = CreateController();

        var result = controller.Complete(3);

        var ok = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        ok.Value.Should().BeEquivalentTo(completed);
    }

    [Fact]
    public void Complete_ShouldReturnNotFound_WhenTodoDoesNotExist()
    {
        _commandHandler
            .Setup(handler => handler.Complete(new CompleteTodoCommand(100)))
            .Returns((TodoItem?)null);
        var controller = CreateController();

        var result = controller.Complete(100);

        result.Result.Should().BeOfType<NotFoundResult>();
    }

    [Fact]
    public void Delete_ShouldReturnNoContent_WhenTodoExists()
    {
        _commandHandler.Setup(handler => handler.Delete(new DeleteTodoCommand(1))).Returns(true);
        var controller = CreateController();

        var result = controller.Delete(1);

        result.Should().BeOfType<NoContentResult>();
    }

    [Fact]
    public void Delete_ShouldReturnNotFound_WhenTodoDoesNotExist()
    {
        _commandHandler.Setup(handler => handler.Delete(new DeleteTodoCommand(9))).Returns(false);
        var controller = CreateController();

        var result = controller.Delete(9);

        result.Should().BeOfType<NotFoundResult>();
    }
}
