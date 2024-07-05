namespace ProjectTasks.DataAccess.CosmosDb;

public class Project : Common.Project, IWithPartitionKey
{
    public string PartitionKey { get; set; }
}
