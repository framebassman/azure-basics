namespace ProjectTasks.Tables.WebApi.Models;

public class TaskResponse
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public int ProjectReferenceId { get; set; }
}
