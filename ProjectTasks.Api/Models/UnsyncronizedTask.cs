using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace ProjectTasks.Api.Models
{
    [Table("unsyncronized_tasks")]
    public class UnsyncronizedTask
    {
        [Column("id")]
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int ProjectReferenceId { get; set; }
        [JsonIgnore]
        public UnsyncronizedProject UnsyncronizedProject { get; set; }
    }
}
