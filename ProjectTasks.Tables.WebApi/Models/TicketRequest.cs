namespace ProjectTasks.Tables.WebApi.Models;

public class TicketRequest
{
    public string Name { get; set; }
    public string Description { get; set; }
    public int ProjectReferenceId { get; set; }
}
