using Microsoft.EntityFrameworkCore;

namespace ProjectTasks.CosmosDb.Models;

public class ApplicationContext : DbContext
{
    public DbSet<Project> Projects { get; set; }

    public ApplicationContext(DbContextOptions options) : base(options)
    {
    }

    public const string PartitionKey = nameof(PartitionKey);

    public static string ComputePartitionKey<T>() =>
        typeof(T).Name;

    public void SetPartitionKey<T>(T entity)
        where T : IContainerEntity =>
        Entry(entity).Property(PartitionKey).CurrentValue =
            ComputePartitionKey<T>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Project>()
            .HasNoDiscriminator()
            .ToContainer(nameof(Projects))
            .HasPartitionKey(entity => entity.PartitionKey)
            .HasKey(entity => new { entity.Id });

        base.OnModelCreating(modelBuilder);
    }
}
