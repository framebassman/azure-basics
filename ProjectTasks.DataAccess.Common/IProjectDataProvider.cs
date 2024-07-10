using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace ProjectTasks.DataAccess.Common;

public interface IProjectDataProvider<T> where T : IProject
{
    Task<IEnumerable<T>> GetAllProjectsAsync(CancellationToken token);
    Task<T> GetFirstOrDefaultAsync(Expression<Func<T, bool>> predicate, CancellationToken token);
    Task CreateAsync(T candidate, CancellationToken token);
}
