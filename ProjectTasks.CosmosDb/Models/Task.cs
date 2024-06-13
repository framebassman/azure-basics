using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace ProjectTasks.CosmosDb.Models;

public class Task : IContainerEntity
{
    public int Id { get; set; }
    public string PartitionKey { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
}
