namespace TodoApi.Application.Todos.Commands;

public record CreateTodoCommand(string Title);
public record UpdateTodoCommand(int Id, string Title, bool IsDone);
public record CompleteTodoCommand(int Id);
public record DeleteTodoCommand(int Id);
