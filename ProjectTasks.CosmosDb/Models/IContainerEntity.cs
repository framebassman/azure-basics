namespace ProjectTasks.CosmosDb.Models;

public interface IContainerEntity
{
    public int Id { get; set; }

    public string PartitionKey { get; set; }
}
