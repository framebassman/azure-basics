using Microsoft.EntityFrameworkCore;

namespace ProjectTasks.DataAccess.CosmosDb;

public class CosmosDbContext(DbContextOptions options) : DbContext(options)
{
    public DbSet<Project> Projects { get; set; }
    public DbSet<Ticket> Tickets { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Project>()
            .HasNoDiscriminator()
            .ToContainer(nameof(Projects))
            .HasPartitionKey(entity => entity.PartitionKey)
            .HasKey(entity => new { entity.Id });

        modelBuilder.Entity<Ticket>()
            .HasNoDiscriminator()
            .ToContainer(nameof(Tickets))
            .HasPartitionKey(entity => entity.PartitionKey)
            .HasKey(entity => new { entity.Id });

        base.OnModelCreating(modelBuilder);
    }
}
