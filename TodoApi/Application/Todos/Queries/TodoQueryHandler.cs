using TodoApi.Models;
using TodoApi.Services;

namespace TodoApi.Application.Todos.Queries;

public class TodoQueryHandler : ITodoQueryHandler
{
    private readonly ITodoService _todoService;

    public TodoQueryHandler(ITodoService todoService)
    {
        _todoService = todoService;
    }

    public IReadOnlyList<TodoItem> GetAll(GetTodosQuery query)
    {
        return _todoService.GetAll();
    }

    public TodoItem? GetById(GetTodoByIdQuery query)
    {
        return _todoService.GetById(query.Id);
    }
}
