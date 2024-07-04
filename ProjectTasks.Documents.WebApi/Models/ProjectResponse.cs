namespace ProjectTasks.Documents.WebApi.Models
{
    public class ProjectResponse
    {
        public int Id { get; set; }
        public string PartitionKey { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public ICollection<TicketResponse> Tickets { get; set; }
    }
}
