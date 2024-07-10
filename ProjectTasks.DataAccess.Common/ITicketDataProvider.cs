using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ProjectTasks.DataAccess.Common;

public interface ITicketDataProvider<T> where T : ITicket
{
    Task<IEnumerable<T>> GetAllTicketsAsync(CancellationToken token);
    Task CreateTicketAsync(T candidate, CancellationToken token);
}
