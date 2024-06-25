using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace ProjectTasks.Sync.Model.Sql
{
    [Table("unsyncronized_projects")]
    public class UnsyncronizedProject
    {
        [Column("id")]
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        [ForeignKey("ProjectReferenceId")]
        public ICollection<UnsyncronizedTask> Tasks { get; set; }
    }
}
