using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace ProjectTasks.DataAccess.Common;

public interface IProjectDataProvider
{
    Task<IEnumerable<IProject>> GetAllProjectsAsync(CancellationToken token);
    Task<IProject> GetFirstOrDefaultProjectAsync(Expression<Func<IProject, bool>> predicate, CancellationToken token);
    Task<IProject> CreateProjectAsync(string name, string code, CancellationToken token);
}
