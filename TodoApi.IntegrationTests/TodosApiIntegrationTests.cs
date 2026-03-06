using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using TodoApi.Models;
using Xunit;

namespace TodoApi.IntegrationTests;

public class TodosApiIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public TodosApiIntegrationTests(WebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetAll_ShouldReturnSeedData()
    {
        var todos = await _client.GetFromJsonAsync<List<TodoItem>>("/api/todos");

        todos.Should().NotBeNull();
        todos!.Count.Should().BeGreaterThanOrEqualTo(2);
        todos.Select(todo => todo.Title).Should().Contain("Learn ASP.NET + Angular");
    }

    [Fact]
    public async Task Create_ThenGetById_ShouldReturnCreatedTodo()
    {
        var createResponse = await _client.PostAsJsonAsync("/api/todos", new CreateTodoRequest
        {
            Title = "Integration test todo"
        });

        createResponse.StatusCode.Should().Be(HttpStatusCode.Created);
        var created = await createResponse.Content.ReadFromJsonAsync<TodoItem>();
        created.Should().NotBeNull();

        var getResponse = await _client.GetAsync($"/api/todos/{created!.Id}");
        getResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var fetched = await getResponse.Content.ReadFromJsonAsync<TodoItem>();
        fetched.Should().NotBeNull();
        fetched!.Title.Should().Be("Integration test todo");
    }

    [Fact]
    public async Task Update_ShouldReturnNotFound_WhenTodoDoesNotExist()
    {
        var response = await _client.PutAsJsonAsync("/api/todos/99999", new UpdateTodoRequest
        {
            Title = "Unknown",
            IsDone = true
        });

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Delete_ShouldReturnNoContent_WhenTodoExists()
    {
        var createResponse = await _client.PostAsJsonAsync("/api/todos", new CreateTodoRequest
        {
            Title = "Delete me"
        });
        var created = await createResponse.Content.ReadFromJsonAsync<TodoItem>();

        var deleteResponse = await _client.DeleteAsync($"/api/todos/{created!.Id}");

        deleteResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }
}
