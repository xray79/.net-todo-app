using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using TodoApi.Controllers;
using TodoApi.Models;
using TodoApi.Services;
using Xunit;

namespace TodoApi.UnitTests;

public class TodosControllerTests
{
    private readonly Mock<ITodoService> _todoService = new();

    [Fact]
    public void GetById_ShouldReturnNotFound_WhenTodoDoesNotExist()
    {
        _todoService.Setup(service => service.GetById(99)).Returns((TodoItem?)null);
        var controller = new TodosController(_todoService.Object);

        var result = controller.GetById(99);

        result.Result.Should().BeOfType<NotFoundResult>();
    }

    [Fact]
    public void Create_ShouldReturnBadRequest_WhenTitleIsBlank()
    {
        var controller = new TodosController(_todoService.Object);

        var result = controller.Create(new CreateTodoRequest { Title = " " });

        result.Result.Should().BeOfType<BadRequestObjectResult>();
        _todoService.Verify(service => service.Add(It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public void Create_ShouldReturnCreatedAtAction_WhenRequestIsValid()
    {
        var created = new TodoItem(10, "Write tests", false);
        _todoService.Setup(service => service.Add("Write tests")).Returns(created);
        var controller = new TodosController(_todoService.Object);

        var result = controller.Create(new CreateTodoRequest { Title = "Write tests" });

        var createdResult = result.Result.Should().BeOfType<CreatedAtActionResult>().Subject;
        createdResult.ActionName.Should().Be(nameof(TodosController.GetById));
        createdResult.Value.Should().BeEquivalentTo(created);
    }

    [Fact]
    public void Update_ShouldReturnNotFound_WhenTodoDoesNotExist()
    {
        _todoService
            .Setup(service => service.Update(123, "Missing", true))
            .Returns((TodoItem?)null);
        var controller = new TodosController(_todoService.Object);

        var result = controller.Update(123, new UpdateTodoRequest { Title = "Missing", IsDone = true });

        result.Result.Should().BeOfType<NotFoundResult>();
    }

    [Fact]
    public void Delete_ShouldReturnNoContent_WhenTodoExists()
    {
        _todoService.Setup(service => service.Delete(1)).Returns(true);
        var controller = new TodosController(_todoService.Object);

        var result = controller.Delete(1);

        result.Should().BeOfType<NoContentResult>();
    }
}
