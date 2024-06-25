using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace ProjectTasks.Sync.Model.Sql
{
    [Table("projects")]
    public class Project
    {
        [Column("id")]
        public int Id { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
    }
}
