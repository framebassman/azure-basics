using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace ProjectTasks.DataAccess.Common;

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
