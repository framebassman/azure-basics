using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProjectTasks.Api.Models;

[Table("projects")]
public class Project
{
    [Column("id")]
    public int Id { get; set; }
}
