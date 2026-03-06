using TodoApi.Models;

namespace TodoApi.Services;

public interface ITodoService
{
    IReadOnlyList<TodoItem> GetAll();
    TodoItem? GetById(int id);
    TodoItem Add(string title);
    TodoItem? Update(int id, string title, bool isDone);
    bool Delete(int id);
}
