namespace ProjectTasks.Sync.Model.CosmosDb
{
    public interface IContainerEntity
    {
        public int Id { get; set; }

        public string PartitionKey { get; set; }
    }
}
