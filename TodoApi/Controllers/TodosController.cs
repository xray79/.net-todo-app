using Microsoft.AspNetCore.Mvc;
using TodoApi.Application.Todos.Commands;
using TodoApi.Application.Todos.Queries;
using TodoApi.Models;

namespace TodoApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TodosController : ControllerBase
{
    private readonly ITodoCommandHandler _commandHandler;
    private readonly ITodoQueryHandler _queryHandler;

    public TodosController(ITodoCommandHandler commandHandler, ITodoQueryHandler queryHandler)
    {
        _commandHandler = commandHandler;
        _queryHandler = queryHandler;
    }

    [HttpGet]
    public ActionResult<IReadOnlyList<TodoItem>> GetAll()
    {
        return Ok(_queryHandler.GetAll(new GetTodosQuery()));
    }

    [HttpGet("{id:int}")]
    public ActionResult<TodoItem> GetById(int id)
    {
        var todo = _queryHandler.GetById(new GetTodoByIdQuery(id));
        if (todo is null)
        {
            return NotFound();
        }

        return Ok(todo);
    }

    [HttpPost]
    public ActionResult<TodoItem> Create([FromBody] CreateTodoRequest request)
    {
        var created = _commandHandler.Create(new CreateTodoCommand(request.Title));
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpPut("{id:int}")]
    public ActionResult<TodoItem> Update(int id, [FromBody] UpdateTodoRequest request)
    {
        var updated = _commandHandler.Update(new UpdateTodoCommand(id, request.Title, request.IsDone));
        if (updated is null)
        {
            return NotFound();
        }

        return Ok(updated);
    }

    [HttpPatch("{id:int}/complete")]
    public ActionResult<TodoItem> Complete(int id)
    {
        var completed = _commandHandler.Complete(new CompleteTodoCommand(id));
        if (completed is null)
        {
            return NotFound();
        }

        return Ok(completed);
    }

    [HttpDelete("{id:int}")]
    public IActionResult Delete(int id)
    {
        var deleted = _commandHandler.Delete(new DeleteTodoCommand(id));
        if (!deleted)
        {
            return NotFound();
        }

        return NoContent();
    }
}
