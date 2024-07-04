using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace ProjectTasks.Tables.WebApi.Models;

[Table("projects")]
public class Project
{
    [Column("id")]
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    public string Name { get; set; }
    public string Code { get; set; }
    [ForeignKey("ProjectReferenceId")]
    public ICollection<Task> Tasks { get; set; }
}
