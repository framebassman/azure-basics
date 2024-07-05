using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ProjectTasks.DataAccess.Common;

namespace ProjectTasks.DataAccess.CosmosDb;

public class Project : IProject, IWithPartitionKey
{
    [Column("id")]
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    public string Name { get; set; }
    public string Code { get; set; }
    [ForeignKey("ProjectReferenceId")]
    public ICollection<ITicket> Tickets { get; set; }
    public string PartitionKey { get; set; }
}
