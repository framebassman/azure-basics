using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ProjectTasks.DataAccess.Common;

namespace ProjectTasks.DataAccess.CosmosDb;

public class CosmosDbDataProvider :
    IProjectDataProvider,
    ITicketDataProvider
{
    private CosmosDbContext _db;

    public CosmosDbDataProvider(CosmosDbContext db)
    {
        _db = db;
        _db.Database.EnsureCreated();
    }

    public async Task<IEnumerable<IProject>> GetAllProjectsAsync(CancellationToken token)
    {
        return await _db.Projects.ToListAsync(token);
    }

    public async Task<IProject> GetFirstOrDefaultProjectAsync(Expression<Func<IProject, bool>> predicate, CancellationToken token)
    {
        return await _db.Projects.FirstOrDefaultAsync(predicate, token);
    }

    public async Task<IProject> CreateProjectAsync(string name, string code, CancellationToken token)
    {
        var count = await _db.Projects.CountAsync(token);
        var toDb = new Project
        {
            Id = count + 1,
            Name = name,
            Code = code,
            PartitionKey = "Test"
        };
        var entity = await _db.Projects.AddAsync(toDb, token);
        await _db.SaveChangesAsync(token);
        return entity.Entity;
    }

    public Task<List<IProject>> GetProjectsToSync(Expression<Func<IProject, bool>> predicate, CancellationToken token)
    {
        throw new NotImplementedException();
    }

    public Task<int> GetLastSynchronizedProjectId(CancellationToken token)
    {
        throw new NotImplementedException();
    }

    public async Task<IEnumerable<ITicket>> GetAllTicketsAsync(CancellationToken token)
    {
        return await _db.Tickets.ToListAsync(token);
    }

    public async Task<ITicket> GetFirstOrDefaultAsync(Expression<Func<ITicket, bool>> predicate, CancellationToken token)
    {
        return await _db.Tickets.FirstOrDefaultAsync(predicate, token);
    }

    public async Task<ITicket> CreateTicketAsync(string name, string description, int projectReferenceId, CancellationToken token)
    {
        var count = await _db.Tickets.CountAsync(token);
        var ticket = new Ticket
        {
            Id = count + 1,
            Name = name,
            Description = description,
            PartitionKey = "Test",
            ProjectReferenceId = projectReferenceId
        };
        var entry = await _db.Tickets.AddAsync(ticket, token);
        await _db.SaveChangesAsync(token);
        return entry.Entity;
    }

    public Task<List<ITicket>> GetTicketsWhereAsync(Expression<Func<ITicket, bool>> predicate, CancellationToken token)
    {
        throw new NotImplementedException();
    }

    public Task<int> GetLastSynchronizedTicketId(CancellationToken token)
    {
        throw new NotImplementedException();
    }

    public async Task<bool> AddProjectsBulk(List<Project> projects, CancellationToken token)
    {
        projects.ForEach(e => e.PartitionKey = "Test");
        await _db.Projects.AddRangeAsync(projects, token);
        await _db.SaveChangesAsync(token);
        return true;
    }
}
