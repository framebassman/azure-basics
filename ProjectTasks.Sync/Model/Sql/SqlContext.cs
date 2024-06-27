using Microsoft.EntityFrameworkCore;

namespace ProjectTasks.Sync.Model.Sql
{
    public class SqlContext : DbContext
    {
        public DbSet<UnsyncronizedProject> UnsyncronizedProjects { get; set; }
        public DbSet<UnsyncronizedTask> UnsyncronizedTasks { get; set; }
        public DbSet<Project> Projects { get; set; }
        public DbSet<Task> Tasks { get; set; }

        public SqlContext(DbContextOptions<SqlContext> options) : base(options)
        {
        }
    }
}
