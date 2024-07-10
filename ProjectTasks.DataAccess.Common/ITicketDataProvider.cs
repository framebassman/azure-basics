using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace ProjectTasks.DataAccess.Common;

public interface ITicketDataProvider<T> where T : ITicket
{
    Task<IEnumerable<T>> GetAllTicketsAsync(CancellationToken token);
    Task<T> GetFirstOrDefaultAsync(Expression<Func<T, bool>> predicate, CancellationToken token);
    Task CreateTicketAsync(T candidate, CancellationToken token);
}
