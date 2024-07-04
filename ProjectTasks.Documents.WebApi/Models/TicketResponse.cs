namespace ProjectTasks.Documents.WebApi.Models
{
    public class TicketResponse
    {
        public int Id { get; set; }
        public string PartitionKey { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int ProjectId { get; set; }
    }
}
