namespace ProjectTasks.DataAccess.CosmosDb;

public class Ticket : Common.Ticket, IWithPartitionKey
{
    public string PartitionKey { get; set; }
}
