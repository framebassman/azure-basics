using Microsoft.EntityFrameworkCore;

namespace ProjectTasks.Sync.Model.Sql
{
    public class SqlContext : DbContext
    {
        public DbSet<UnsyncronizedProject> UnsyncronizedProjects { get; set; }
        public DbSet<UnsyncronizedTask> UnsyncronizedTasks { get; set;}

        public SqlContext(DbContextOptions options) : base(options)
        {
        }
    }
}
