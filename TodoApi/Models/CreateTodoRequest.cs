using System.ComponentModel.DataAnnotations;

namespace TodoApi.Models;

public class CreateTodoRequest
{
    [Required]
    public string Title { get; set; } = string.Empty;
}
