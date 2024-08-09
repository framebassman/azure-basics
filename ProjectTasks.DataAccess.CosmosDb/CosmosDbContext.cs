using Microsoft.EntityFrameworkCore;

namespace ProjectTasks.DataAccess.CosmosDb;

public class CosmosDbContext(DbContextOptions<CosmosDbContext> options) : DbContext(options)
{
    public DbSet<Project> Projects { get; set; }
    public DbSet<Ticket> Tickets { get; set; }
    public DbSet<Settings> Settings { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Project>()
            .HasNoDiscriminator()
            .ToContainer(nameof(Projects))
            .HasPartitionKey(entity => entity.PartitionKey);

        modelBuilder.Entity<Ticket>()
            .HasNoDiscriminator()
            .ToContainer(nameof(Tickets))
            .HasPartitionKey(entity => entity.PartitionKey);

        modelBuilder.Entity<Settings>()
            .HasNoDiscriminator()
            .ToContainer(nameof(Settings))
            .HasPartitionKey(entity => entity.PartitionKey);

        base.OnModelCreating(modelBuilder);
    }
}
