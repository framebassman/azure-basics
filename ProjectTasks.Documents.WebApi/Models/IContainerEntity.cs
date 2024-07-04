namespace ProjectTasks.Documents.WebApi.Models;

public interface IContainerEntity
{
    public int Id { get; set; }

    public string PartitionKey { get; set; }
}
