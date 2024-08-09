using System.ComponentModel.DataAnnotations;

namespace ProjectTasks.DataAccess.CosmosDb;

public class Settings : IWithPartitionKey
{
    [Key]
    public string Key { get; set; }
    public string Value { get; set; }
    public string PartitionKey { get; set; }
    public static string LastSynchronizedProjectId = "LastSynchronizedProjectId";
    public static string LastSynchronizedTicketId = "LastSynchronizedTaskId";
}
