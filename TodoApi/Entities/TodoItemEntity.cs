namespace TodoApi.Entities;

public class TodoItemEntity
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public bool IsDone { get; set; }
    public string OwnerId { get; set; } = string.Empty;
}
