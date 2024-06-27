using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace ProjectTasks.Api.Models
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
        public ICollection<UnsyncronizedTask> UnsyncronizedTasks { get; set; }
    }
}
