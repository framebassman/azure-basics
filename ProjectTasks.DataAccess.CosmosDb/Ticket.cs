namespace ProjectTasks.DataAccess.CosmosDb;

public class Ticket : ProjectTasks.DataAccess.Common.Ticket, IWithPartitionKey
{
    public string PartitionKey { get; set; }
}
