using Microsoft.EntityFrameworkCore;

namespace ProjectTasks.Api.Models;

public class ApplicationContext : DbContext
{
    public DbSet<Project> Projects { get; set; }
    public DbSet<Task> Tasks { get; set; }

    public ApplicationContext(DbContextOptions options) : base(options)
    {
    }
}
