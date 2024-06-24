namespace ProjectTasks.CosmosDb.Models
{
    public class Project : IContainerEntity
    {
        public int Id { get; set; }

        public string PartitionKey { get; set; }

        public string Name { get; set; }
        public string Code { get; set; }

        public ICollection<Task> Tasks { get; set; }
    }
}
