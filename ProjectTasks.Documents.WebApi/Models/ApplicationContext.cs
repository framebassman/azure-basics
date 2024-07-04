using Microsoft.EntityFrameworkCore;

namespace ProjectTasks.Documents.WebApi.Models;

public class ApplicationContext : DbContext
{
    public DbSet<Project> Projects { get; set; }
    public DbSet<Task> Tasks { get; set; }

    public ApplicationContext(DbContextOptions options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Project>()
            .HasNoDiscriminator()
            .ToContainer(nameof(Projects))
            .HasPartitionKey(entity => entity.PartitionKey)
            .HasKey(entity => new { entity.Id });

        modelBuilder.Entity<Task>()
            .HasNoDiscriminator()
            .ToContainer(nameof(Tasks))
            .HasPartitionKey(entity => entity.PartitionKey)
            .HasKey(entity => new { entity.Id });

        base.OnModelCreating(modelBuilder);
    }
}
