using Microsoft.EntityFrameworkCore;

namespace ProjectTasks.DataAccess.AzureSQL;

public class AzureSqlDbContext : DbContext
{
    public DbSet<Project> Projects { get; set; }
    public DbSet<Ticket> Tickets { get; set; }
    public DbSet<Settings> Settings { get; set; }

    public AzureSqlDbContext(DbContextOptions<AzureSqlDbContext> options) : base(options)
    {}
}
