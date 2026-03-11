using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using TodoApi.Auth;
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
    public async Task GetAll_ShouldReturnUnauthorized_WhenNoTokenIsProvided()
    {
        var response = await _client.GetAsync("/api/todos");

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Register_ThenLogin_ShouldReturnToken()
    {
        var email = $"user-{Guid.NewGuid():N}@example.com";

        var registerResponse = await _client.PostAsJsonAsync("/api/auth/register", new RegisterRequest
        {
            Email = email,
            Password = "password123"
        });

        registerResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var registered = await registerResponse.Content.ReadFromJsonAsync<AuthResponse>();
        registered.Should().NotBeNull();
        registered!.Token.Should().NotBeNullOrWhiteSpace();

        var loginResponse = await _client.PostAsJsonAsync("/api/auth/login", new LoginRequest
        {
            Email = email,
            Password = "password123"
        });

        loginResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var loggedIn = await loginResponse.Content.ReadFromJsonAsync<AuthResponse>();
        loggedIn.Should().NotBeNull();
        loggedIn!.Token.Should().NotBeNullOrWhiteSpace();
    }

    [Fact]
    public async Task AuthenticatedUser_CanCreateAndReadOwnTodo()
    {
        var token = await RegisterAndLoginAsync();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var createResponse = await _client.PostAsJsonAsync("/api/todos", new CreateTodoRequest
        {
            Title = "Authenticated integration todo"
        });

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

        var fetched = await getResponse.Content.ReadFromJsonAsync<TodoItem>();
        fetched.Should().NotBeNull();
        fetched!.Title.Should().Be("Authenticated integration todo");
    }

    private async Task<string> RegisterAndLoginAsync()
    {
        var email = $"user-{Guid.NewGuid():N}@example.com";
        const string password = "password123";

        await _client.PostAsJsonAsync("/api/auth/register", new RegisterRequest
        {
            Email = email,
            Password = password
        });

        var loginResponse = await _client.PostAsJsonAsync("/api/auth/login", new LoginRequest
        {
            Email = email,
            Password = password
        });

        var auth = await loginResponse.Content.ReadFromJsonAsync<AuthResponse>();
        return auth!.Token;
    }
}
