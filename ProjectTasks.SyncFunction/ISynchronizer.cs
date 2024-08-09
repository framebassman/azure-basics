using System.Threading;
using System.Threading.Tasks;

namespace ProjectTasks.SyncFunction;

public interface ISynchronizer
{
    Task<bool> SynchronizeAsync(CancellationToken token);
}
