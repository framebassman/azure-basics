using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ProjectTasks.DataAccess.Common;

namespace ProjectTasks.DataAccess.AzureSQL;

public class AzureSqlDataProvider :
    IProjectDataProvider<Project>,
    ITicketDataProvider<Ticket>
{
    private AzureSqlDbContext _db;

    public AzureSqlDataProvider(AzureSqlDbContext db)
    {
        _db = db;
    }

    public async Task<IEnumerable<Project>> GetAllProjectsAsync(CancellationToken token)
    {
        return await _db.Projects.ToListAsync(token);
    }

    public async Task<Project> GetFirstOrDefaultAsync(Expression<Func<Project, bool>> predicate, CancellationToken token)
    {
        return await _db.Projects.FirstOrDefaultAsync(predicate, token);
    }

    public async Task CreateAsync(Project project, CancellationToken token)
    {
        await _db.Projects.AddAsync(project, token);
        await _db.SaveChangesAsync(token);
    }

    public async Task<IEnumerable<Ticket>> GetAllTicketsAsync(CancellationToken token)
    {
        return await _db.Tasks.ToListAsync(token);
    }

    public async Task<Ticket> GetFirstOrDefaultAsync(Expression<Func<Ticket, bool>> predicate, CancellationToken token)
    {
        return await _db.Tasks.FirstOrDefaultAsync(predicate, token);
    }

    public async Task CreateTicketAsync(Ticket ticket, CancellationToken token)
    {
        await _db.Tasks.AddAsync(ticket, token);
        await _db.SaveChangesAsync(token);
    }
}
