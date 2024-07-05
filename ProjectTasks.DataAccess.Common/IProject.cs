using System.Collections.Generic;

namespace ProjectTasks.DataAccess.Common;

public interface IProject
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Code { get; set; }
}
