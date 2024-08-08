using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProjectTasks.DataAccess.AzureSQL;

[Table("settings")]
public class Settings
{
    [Key]
    public string Key { get; set; }
    public string Value { get; set; }
    public static string LastSynchronizedProjectId = "LastSynchronizedProjectId";
    public static string LastSynchronizedTicketId = "LastSynchronizedTaskId";
}
