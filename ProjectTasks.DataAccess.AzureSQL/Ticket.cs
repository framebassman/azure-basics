using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace ProjectTasks.DataAccess.AzureSQL;

[Table("tickets")]
public class Ticket
{
    [Column("id")]
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public int ProjectReferenceId { get; set; }
    [JsonIgnore]
    public Project Project { get; set; }
}

