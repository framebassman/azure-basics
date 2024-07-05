using System.ComponentModel.DataAnnotations;
using ProjectTasks.DataAccess.Common;

namespace ProjectTasks.DataAccess.CosmosDb;

public class Ticket : ITicket, IWithPartitionKey
{
    [Key]
    public int Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public int ProjectReferenceId { get; set; }
    public string PartitionKey { get; set; }
}
