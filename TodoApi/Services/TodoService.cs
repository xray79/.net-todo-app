using Microsoft.EntityFrameworkCore;
using TodoApi.Data;
using TodoApi.Entities;
using TodoApi.Models;

namespace TodoApi.Services;

public class TodoService : ITodoService
{
    private readonly AppDbContext _dbContext;

    public TodoService(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IReadOnlyList<TodoItem>> GetAllAsync(string ownerId)
    {
        return await _dbContext.Todos
            .Where(todo => todo.OwnerId == ownerId)
            .OrderBy(todo => todo.Id)
            .Select(todo => new TodoItem(todo.Id, todo.Title, todo.IsDone))
            .ToListAsync();
    }

    public async Task<TodoItem?> GetByIdAsync(string ownerId, int id)
    {
        var todo = await _dbContext.Todos
            .FirstOrDefaultAsync(item => item.OwnerId == ownerId && item.Id == id);

        return todo is null ? null : Map(todo);
    }

    public async Task<TodoItem> AddAsync(string ownerId, string title)
    {
        var todo = new TodoItemEntity
        {
            OwnerId = ownerId,
            Title = title.Trim(),
            IsDone = false
        };

        _dbContext.Todos.Add(todo);
        await _dbContext.SaveChangesAsync();

        return Map(todo);
    }

    public async Task<TodoItem?> UpdateAsync(string ownerId, int id, string title, bool isDone)
    {
        var todo = await _dbContext.Todos
            .FirstOrDefaultAsync(item => item.OwnerId == ownerId && item.Id == id);

        if (todo is null)
        {
            return null;
        }

        todo.Title = title.Trim();
        todo.IsDone = isDone;
        await _dbContext.SaveChangesAsync();

        return Map(todo);
    }

    public async Task<bool> DeleteAsync(string ownerId, int id)
    {
        var todo = await _dbContext.Todos
            .FirstOrDefaultAsync(item => item.OwnerId == ownerId && item.Id == id);

        if (todo is null)
        {
            return false;
        }

        _dbContext.Todos.Remove(todo);
        await _dbContext.SaveChangesAsync();
        return true;
    }

    private static TodoItem Map(TodoItemEntity entity) => new(entity.Id, entity.Title, entity.IsDone);
}
