namespace ProjectTasks.DataAccess.CosmosDb;

public interface IWithPartitionKey
{
    public string PartitionKey { get; set; }
}
