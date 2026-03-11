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
    public async Task CreateTodo_ShouldReturnCreatedTodo()
    {
        var title = $"Integration create {Guid.NewGuid():N}";

        var createResponse = await _client.PostAsJsonAsync("/api/todos", new CreateTodoRequest { Title = title });

        createResponse.StatusCode.Should().Be(HttpStatusCode.Created);
        var created = await createResponse.Content.ReadFromJsonAsync<TodoItem>();
        created.Should().NotBeNull();
        created!.Title.Should().Be(title);
        created.IsDone.Should().BeFalse();
    }

    [Fact]
    public async Task GetTodos_ShouldIncludeCreatedTodo()
    {
        var title = $"Integration list {Guid.NewGuid():N}";
        await _client.PostAsJsonAsync("/api/todos", new CreateTodoRequest { Title = title });

        var todos = await _client.GetFromJsonAsync<List<TodoItem>>("/api/todos");

        todos.Should().NotBeNull();
        todos!.Select(todo => todo.Title).Should().Contain(title);
    }

    [Fact]
    public async Task UpdateTodo_ShouldPersistChanges()
    {
        var created = await CreateTodoAsync($"Integration update {Guid.NewGuid():N}");

        var updateResponse = await _client.PutAsJsonAsync($"/api/todos/{created.Id}", new UpdateTodoRequest
        {
            Title = "Updated title",
            IsDone = true
        });

        updateResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var updated = await updateResponse.Content.ReadFromJsonAsync<TodoItem>();
        updated.Should().NotBeNull();
        updated!.Title.Should().Be("Updated title");
        updated.IsDone.Should().BeTrue();
    }

    [Fact]
    public async Task CompleteTodo_ShouldMarkItemAsDone()
    {
        var created = await CreateTodoAsync($"Integration complete {Guid.NewGuid():N}");

        var completeResponse = await _client.PatchAsync($"/api/todos/{created.Id}/complete", content: null);

        completeResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var completed = await completeResponse.Content.ReadFromJsonAsync<TodoItem>();
        completed.Should().NotBeNull();
        completed!.IsDone.Should().BeTrue();
    }

    [Fact]
    public async Task DeleteTodo_ShouldRemoveItem()
    {
        var created = await CreateTodoAsync($"Integration delete {Guid.NewGuid():N}");

        var deleteResponse = await _client.DeleteAsync($"/api/todos/{created.Id}");
        deleteResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);

        var getResponse = await _client.GetAsync($"/api/todos/{created.Id}");
        getResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    private async Task<TodoItem> CreateTodoAsync(string title)
    {
        var response = await _client.PostAsJsonAsync("/api/todos", new CreateTodoRequest { Title = title });
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        return (await response.Content.ReadFromJsonAsync<TodoItem>())!;
    }
}
