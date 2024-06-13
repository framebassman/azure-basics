using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using System.Text.Json;

namespace ProjectTasks.CosmosDb.Models;

public class Project : IContainerEntity
{
    public int Id { get; set; }
    public string PartitionKey { get; set; }
    public string Name { get; set; }
    public string Code { get; set; }
    // public Task[] Tasks { get; set; }
    // [ForeignKey("ProjectReferenceId")]
    public ICollection<Task> Tasks { get; set; }
    public override string ToString()
    {
        return JsonSerializer.Serialize(this);
    }
}
