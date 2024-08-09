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

    public async Task<bool> AddProjectsBulk(List<Project> projects, CancellationToken token)
    {
        projects.ForEach(e => e.PartitionKey = "Test");
        await _db.Projects.AddRangeAsync(projects, token);
        await _db.SaveChangesAsync(token);
        return true;
    }

    public async Task<bool> AddTicketsBulk(List<Ticket> projects, CancellationToken token)
    {
        projects.ForEach(e => e.PartitionKey = "Test");
        await _db.Tickets.AddRangeAsync(projects, token);
        await _db.SaveChangesAsync(token);
        return true;
    }

    public async Task<int> GetLastSynchronizedProjectId(CancellationToken token)
    {
        int.TryParse(await GetSetting(Settings.LastSynchronizedProjectId, token), out var candidate);
        return candidate;
    }

    public async Task<int> GetLastSynchronizedTicketId(CancellationToken token)
    {
        int.TryParse(await GetSetting(Settings.LastSynchronizedTicketId, token), out var candidate);
        return candidate;
    }

    private async Task<string> GetSetting(string key, CancellationToken token)
    {
        var candidate = await _db.Settings.FirstOrDefaultAsync(entry => entry.Key == key, token);
        return candidate == null ? "" : candidate.Value;
    }

    private async Task<bool> SetSetting(string key, string value, CancellationToken token)
    {
        var current = await _db.Settings.FirstOrDefaultAsync(entry => entry.Key == key, token);
        if (current == null)
        {
            await _db.Settings.AddAsync(new Settings{ Key = key, Value = value, PartitionKey = "Test" }, token);
            await _db.SaveChangesAsync(token);
            return true;
        }

        current.Value = value;
        await _db.SaveChangesAsync(token);
        return true;
    }

    public async Task<bool> UpdateLastSynchronizedProjectId(int id, CancellationToken token)
    {
        await SetSetting(Settings.LastSynchronizedProjectId, id.ToString(), token);
        return true;
    }

    public async Task<bool> UpdateLastSynchronizedTicketId(int id, CancellationToken token)
    {
        await SetSetting(Settings.LastSynchronizedTicketId, id.ToString(), token);
        return true;
    }
}
