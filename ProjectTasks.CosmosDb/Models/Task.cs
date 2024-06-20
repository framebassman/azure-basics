using System.Text.Json.Serialization;

namespace ProjectTasks.CosmosDb.Models
{
    public class Task : IContainerEntity
    {
        public int Id { get; set; }

        public string PartitionKey { get; set; }

        public int ProjectId { get; set; }

        public Project Project { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }
    }
}
