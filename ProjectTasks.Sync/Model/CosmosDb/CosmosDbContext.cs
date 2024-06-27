using Microsoft.EntityFrameworkCore;
using ProjectTasks.Sync.Model.CosmosDb;
using Task = ProjectTasks.Sync.Model.CosmosDb.Task;

namespace ProjectTasks.Sync.Model.CosmosDb
{
    public class CosmosDbContext : DbContext
    {
        public DbSet<Project> Projects { get; set; }
        public DbSet<Task> Tasks { get; set; }

        public CosmosDbContext(DbContextOptions<CosmosDbContext> options) : base(options)
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
}
