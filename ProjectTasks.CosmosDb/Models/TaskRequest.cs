namespace ProjectTasks.CosmosDb.Models;

public class TaskRequest
{
    public string Name { get; set; }
    public string Description { get; set; }
    public int ProjectReferenceId { get; set; }
}
