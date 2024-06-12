using Microsoft.EntityFrameworkCore;

namespace ProjectTasks.CosmosDb.Models;

public class ApplicationContext : DbContext
{
    public ApplicationContext(DbContextOptions options) : base(options)
    {
    }
}
