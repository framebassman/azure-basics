using System.Text.Json.Serialization;

namespace ProjectTasks.DataAccess.Common;

public interface ITicket
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public int ProjectReferenceId { get; set; }
    [JsonIgnore]
    public IProject Project { get; set; }
}
