using TodoApi.Models;

namespace TodoApi.Services;

public interface ITodoService
{
    Task<IReadOnlyList<TodoItem>> GetAllAsync(string ownerId);
    Task<TodoItem?> GetByIdAsync(string ownerId, int id);
    Task<TodoItem> AddAsync(string ownerId, string title);
    Task<TodoItem?> UpdateAsync(string ownerId, int id, string title, bool isDone);
    Task<bool> DeleteAsync(string ownerId, int id);
}
