using TodoApi.Models;

namespace TodoApi.Application.Todos.Queries;

public interface ITodoQueryHandler
{
    IReadOnlyList<TodoItem> GetAll(GetTodosQuery query);
    TodoItem? GetById(GetTodoByIdQuery query);
}
