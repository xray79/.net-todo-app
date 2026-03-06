using System.ComponentModel.DataAnnotations;

namespace TodoApi.Models;

public class UpdateTodoRequest
{
    [Required]
    public string Title { get; set; } = string.Empty;

    public bool IsDone { get; set; }
}
