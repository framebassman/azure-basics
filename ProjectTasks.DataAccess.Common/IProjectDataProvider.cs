using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ProjectTasks.DataAccess.Common;

public interface IProjectDataProvider<T> where T : IProject
{
    Task<IEnumerable<T>> GetAllProjectsAsync(CancellationToken token);
    Task CreateProjectAsync(T candidate, CancellationToken token);
}
