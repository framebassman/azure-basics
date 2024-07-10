using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ProjectTasks.DataAccess.Common;

namespace ProjectTasks.DataAccess.AzureSQL;

public class AzureSqlProjectDataService :
    IProjectDataProvider<Project>,
    ITicketDataProvider<Ticket>
{
    private AzureSqlDbContext _db;

    public AzureSqlProjectDataService(AzureSqlDbContext db)
    {
        _db = db;
    }

    public async Task<IEnumerable<Project>> GetAllProjectsAsync(CancellationToken token)
    {
        return await _db.Projects.ToListAsync(token);
    }

    public async Task CreateProjectAsync(Project project, CancellationToken token)
    {
        await _db.Projects.AddAsync(project, token);
        await _db.SaveChangesAsync(token);
    }

    public async Task<IEnumerable<Ticket>> GetAllTicketsAsync(CancellationToken token)
    {
        return await _db.Tasks.ToListAsync(token);
    }

    public async Task CreateTicketAsync(Ticket ticket, CancellationToken token)
    {
        await _db.Tasks.AddAsync(ticket, token);
        await _db.SaveChangesAsync(token);
    }
}
