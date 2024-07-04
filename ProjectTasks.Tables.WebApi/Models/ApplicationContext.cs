using Microsoft.EntityFrameworkCore;

namespace ProjectTasks.DataAccess.Common;

public class ApplicationContext : DbContext
{
    public DbSet<Project> Projects { get; set; }
    public DbSet<Ticket> Tasks { get; set; }

    public ApplicationContext(DbContextOptions options) : base(options)
    {
    }
}
