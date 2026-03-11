using System.ComponentModel.DataAnnotations;

namespace TodoApi.Models;

public class UpdateTodoRequest
{
    [Required]
    [StringLength(200, MinimumLength = 1)]
    public string Title { get; set; } = string.Empty;

    public bool IsDone { get; set; }
}
