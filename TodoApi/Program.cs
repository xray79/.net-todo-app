var builder = WebApplication.CreateBuilder(args);

builder.WebHost.UseUrls("http://localhost:5080");

builder.Services.AddCors(options =>
{
    options.AddPolicy("AngularDev", policy =>
    {
        policy.WithOrigins("http://localhost:4200")
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

var app = builder.Build();
app.UseCors("AngularDev");

var todos = new List<TodoItem>
{
    new(1, "Learn ASP.NET + Angular", false),
    new(2, "Build a Todo UI", true)
};

var nextId = todos.Max(t => t.Id) + 1;

app.MapGet("/api/todos", () => Results.Ok(todos));

app.MapPost("/api/todos", (CreateTodoRequest request) =>
{
    if (string.IsNullOrWhiteSpace(request.Title))
    {
        return Results.BadRequest(new { error = "Title is required." });
    }

    var todo = new TodoItem(nextId++, request.Title.Trim(), false);
    todos.Add(todo);
    return Results.Created($"/api/todos/{todo.Id}", todo);
});

app.MapPut("/api/todos/{id:int}", (int id, UpdateTodoRequest request) =>
{
    var index = todos.FindIndex(t => t.Id == id);
    if (index == -1)
    {
        return Results.NotFound();
    }

    if (string.IsNullOrWhiteSpace(request.Title))
    {
        return Results.BadRequest(new { error = "Title is required." });
    }

    var current = todos[index];
    var updated = current with
    {
        Title = request.Title.Trim(),
        IsDone = request.IsDone
    };

    todos[index] = updated;
    return Results.Ok(updated);
});

app.MapDelete("/api/todos/{id:int}", (int id) =>
{
    var removed = todos.RemoveAll(t => t.Id == id);
    return removed == 0 ? Results.NotFound() : Results.NoContent();
});

app.Run();

public record TodoItem(int Id, string Title, bool IsDone);
public record CreateTodoRequest(string Title);
public record UpdateTodoRequest(string Title, bool IsDone);
