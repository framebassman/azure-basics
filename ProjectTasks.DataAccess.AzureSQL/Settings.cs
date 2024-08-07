using System.ComponentModel.DataAnnotations;

namespace ProjectTasks.DataAccess.AzureSQL;

public class Settings
{
    [Key]
    public string Key { get; set; }
    public string Value { get; set; }
    public static string LastSynchronizedProjectId = "LastSynchronizedProjectId";
    public static string LastSynchronizedTicketId = "LastSynchronizedTaskId";
}
