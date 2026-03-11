using TodoApi.Application.Todos.Commands;
using TodoApi.Application.Todos.Queries;
using TodoApi.Infrastructure.Events;
using TodoApi.Services;
using TodoApi.Application.Events;

var builder = WebApplication.CreateBuilder(args);

if (string.IsNullOrWhiteSpace(Environment.GetEnvironmentVariable("ASPNETCORE_URLS")))
{
    builder.WebHost.UseUrls("http://localhost:5080");
}

builder.Services.AddControllers();
builder.Services.AddHealthChecks();
builder.Services.AddSingleton<ITodoService, InMemoryTodoService>();
builder.Services.AddScoped<ITodoCommandHandler, TodoCommandHandler>();
builder.Services.AddScoped<ITodoQueryHandler, TodoQueryHandler>();
builder.Services.AddSingleton<IEventPublisher, LoggingEventPublisher>();

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
app.MapControllers();
app.MapHealthChecks("/health");

app.Run();

public partial class Program;
