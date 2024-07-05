using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProjectTasks.DataAccess.AzureSQL;

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
    public ICollection<Ticket> Tickets { get; set; }
}

