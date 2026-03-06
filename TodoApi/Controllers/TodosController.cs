using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TodoApi.Models;
using TodoApi.Services;

namespace TodoApi.Controllers;

[ApiController]
[Authorize]
[Route("api/[controller]")]
public class TodosController : ControllerBase
{
    private readonly ITodoService _todoService;

    public TodosController(ITodoService todoService)
    {
        _todoService = todoService;
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<TodoItem>>> GetAll()
    {
        var todos = await _todoService.GetAllAsync(GetUserId());
        return Ok(todos);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<TodoItem>> GetById(int id)
    {
        var todo = await _todoService.GetByIdAsync(GetUserId(), id);
        if (todo is null)
        {
            return NotFound();
        }

        return Ok(todo);
    }

    [HttpPost]
    public async Task<ActionResult<TodoItem>> Create([FromBody] CreateTodoRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Title))
        {
            return BadRequest(new { error = "Title is required." });
        }

        var created = await _todoService.AddAsync(GetUserId(), request.Title);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<TodoItem>> Update(int id, [FromBody] UpdateTodoRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Title))
        {
            return BadRequest(new { error = "Title is required." });
        }

        var updated = await _todoService.UpdateAsync(GetUserId(), id, request.Title, request.IsDone);
        if (updated is null)
        {
            return NotFound();
        }

        return Ok(updated);
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var deleted = await _todoService.DeleteAsync(GetUserId(), id);
        if (!deleted)
        {
            return NotFound();
        }

        return NoContent();
    }

    private string GetUserId()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId))
        {
            throw new UnauthorizedAccessException("User identifier claim is missing.");
        }

        return userId;
    }
}
