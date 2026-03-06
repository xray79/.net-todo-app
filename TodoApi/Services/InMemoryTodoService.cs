using TodoApi.Models;

namespace TodoApi.Services;

public class InMemoryTodoService : ITodoService
{
    private readonly List<TodoItem> _todos =
    [
        new(1, "Learn ASP.NET + Angular", false),
        new(2, "Build a Todo UI", true)
    ];

    private int _nextId = 3;

    public IReadOnlyList<TodoItem> GetAll() => _todos;

    public TodoItem? GetById(int id) => _todos.FirstOrDefault(todo => todo.Id == id);

    public TodoItem Add(string title)
    {
        var todo = new TodoItem(_nextId++, title.Trim(), false);
        _todos.Add(todo);
        return todo;
    }

    public TodoItem? Update(int id, string title, bool isDone)
    {
        var index = _todos.FindIndex(todo => todo.Id == id);
        if (index == -1)
        {
            return null;
        }

        var updated = _todos[index] with
        {
            Title = title.Trim(),
            IsDone = isDone
        };

        _todos[index] = updated;
        return updated;
    }

    public bool Delete(int id)
    {
        var removed = _todos.RemoveAll(todo => todo.Id == id);
        return removed > 0;
    }
}
