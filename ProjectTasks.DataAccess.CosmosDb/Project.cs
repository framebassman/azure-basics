using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using ProjectTasks.DataAccess.Common;

namespace ProjectTasks.DataAccess.CosmosDb;

public class Project : IProject, IWithPartitionKey
{
    [Key]
    public int Id { get; set; }
    public string Name { get; set; }
    public string Code { get; set; }
    public ICollection<Ticket>? Tickets { get; set; }
    public string PartitionKey { get; set; }
}
