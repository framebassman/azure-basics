using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ProjectTasks.DataAccess.Common;

namespace ProjectTasks.DataAccess.AzureSQL;

[Table("projects")]
public class Project : IProject
{
    [Column("id")]
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    public string Name { get; set; }
    public string Code { get; set; }
    public DateTime WasSynchronizedAt { get; set; }
    [ForeignKey("ProjectReferenceId")]
    public ICollection<Ticket> Tickets { get; set; }
}
