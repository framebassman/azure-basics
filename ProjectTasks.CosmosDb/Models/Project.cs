using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace ProjectTasks.CosmosDb.Models;

public class Project : IContainerEntity
{
    public int Id { get; set; }

    public string PartitionKey { get; set; }

    public string Name { get; set; }
    // [ForeignKey("ProjectReferenceId")]
    // public ICollection<Task> Tasks { get; set; }
}
