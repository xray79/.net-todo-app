using TodoApi.Models;

namespace TodoApi.Application.Todos.Commands;

public interface ITodoCommandHandler
{
    TodoItem Create(CreateTodoCommand command);
    TodoItem? Update(UpdateTodoCommand command);
    TodoItem? Complete(CompleteTodoCommand command);
    bool Delete(DeleteTodoCommand command);
}
