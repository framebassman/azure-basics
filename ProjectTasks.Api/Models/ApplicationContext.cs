using Microsoft.EntityFrameworkCore;

namespace ProjectTasks.Api.Models;

public class ApplicationContext : DbContext
{
    public DbSet<Project> Projects { get; set; }
    
    protected override void OnConfiguring(DbContextOptionsBuilder builder)
    {
        builder.UseInMemoryDatabase(databaseName: "Data");
    }
}