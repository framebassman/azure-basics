using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace ProjectTasks.DataAccess.Common;

public interface ITicketDataProvider
{
    Task<IEnumerable<ITicket>> GetAllTicketsAsync(CancellationToken token);
    Task<ITicket> GetFirstOrDefaultAsync(Expression<Func<ITicket, bool>> predicate, CancellationToken token);
    Task<ITicket> CreateTicketAsync(string name, string description, int projectReferenceId, CancellationToken token);
    Task<List<ITicket>> GetTicketsWhereAsync(Expression<Func<ITicket, bool>> predicate, CancellationToken token);
    Task<int> GetLastSynchronizedTicketId(CancellationToken token);
}
