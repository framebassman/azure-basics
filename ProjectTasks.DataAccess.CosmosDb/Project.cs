namespace ProjectTasks.DataAccess.CosmosDb;

public class Project : ProjectTasks.DataAccess.Common.Project, IWithPartitionKey
{
    public string PartitionKey { get; set; }
}
