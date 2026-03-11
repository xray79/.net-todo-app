using System.Security.Claims;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
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
    public async Task GetById_ShouldReturnNotFound_WhenTodoDoesNotExist()
    {
        const string userId = "user-1";
        _todoService.Setup(service => service.GetByIdAsync(userId, 99)).ReturnsAsync((TodoItem?)null);
        var controller = CreateController(userId);

        var result = await controller.GetById(99);

        var result = controller.GetAll();

        var ok = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        ok.Value.Should().BeEquivalentTo(todos);
    }

    [Fact]
    public async Task Create_ShouldReturnBadRequest_WhenTitleIsBlank()
    {
        var controller = CreateController("user-1");

        var result = await controller.Create(new CreateTodoRequest { Title = " " });

        result.Result.Should().BeOfType<BadRequestObjectResult>();
        _todoService.Verify(service => service.AddAsync(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public async Task Create_ShouldReturnCreatedAtAction_WhenRequestIsValid()
    {
        const string userId = "user-1";
        var created = new TodoItem(10, "Write tests", false);
        _todoService.Setup(service => service.AddAsync(userId, "Write tests")).ReturnsAsync(created);
        var controller = CreateController(userId);

        var result = await controller.Create(new CreateTodoRequest { Title = "Write tests" });

        var createdResult = result.Result.Should().BeOfType<CreatedAtActionResult>().Subject;
        createdResult.ActionName.Should().Be(nameof(TodosController.GetById));
        createdResult.Value.Should().BeEquivalentTo(created);
    }

    [Fact]
    public async Task Update_ShouldReturnNotFound_WhenTodoDoesNotExist()
    {
        const string userId = "user-1";
        _todoService
            .Setup(service => service.UpdateAsync(userId, 123, "Missing", true))
            .ReturnsAsync((TodoItem?)null);
        var controller = CreateController(userId);

        var result = await controller.Update(123, new UpdateTodoRequest { Title = "Missing", IsDone = true });

        result.Result.Should().BeOfType<NotFoundResult>();
    }

    [Fact]
    public async Task Delete_ShouldReturnNoContent_WhenTodoExists()
    {
        const string userId = "user-1";
        _todoService.Setup(service => service.DeleteAsync(userId, 1)).ReturnsAsync(true);
        var controller = CreateController(userId);

        var result = await controller.Delete(1);

        result.Should().BeOfType<NoContentResult>();
    }

    private TodosController CreateController(string userId)
    {
        var controller = new TodosController(_todoService.Object);

        controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext
            {
                User = new ClaimsPrincipal(
                    new ClaimsIdentity([new Claim(ClaimTypes.NameIdentifier, userId)], "TestAuth"))
            }
        };

        return controller;
    }
}
