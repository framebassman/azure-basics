using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ProjectTasks.DataAccess.Common;

namespace ProjectTasks.DataAccess.AzureSQL;

public class AzureSqlDataProvider :
    IProjectDataProvider,
    ITicketDataProvider
{
    private AzureSqlDbContext _db;

    public AzureSqlDataProvider(AzureSqlDbContext db)
    {
        _db = db;
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
        var toDb = new Project
        {
            Name = name,
            Code = code
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
        var ticket = new Ticket
        {
            Name = name,
            Description = description,
            ProjectReferenceId = projectReferenceId
        };
        var entry = await _db.Tickets.AddAsync(ticket, token);
        await _db.SaveChangesAsync(token);
        return entry.Entity;
    }

    private async Task<string> GetSetting(string key)
    {
        var candidate = await _db.Settings.FirstOrDefaultAsync(entry => entry.Key == key);
        return candidate == null ? "" : candidate.Value;
    }

    private async Task<bool> SetSetting(string key, string value, CancellationToken token)
    {
        var current = await _db.Settings.FirstOrDefaultAsync(entry => entry.Key == key, token);
        if (current == null)
        {
            await _db.Settings.AddAsync(new Settings{ Key = key, Value = value }, token);
            await _db.SaveChangesAsync(token);
            return true;
        }

        current.Value = value;
        await _db.SaveChangesAsync(token);
        return true;
    }

    // public async Task<IProject> GetUnsynchronizedProjects()
    // {
    //     var fromConfig = await GetSetting(Settings.LastSynchronizedProjectId);
    //     bool success = int.TryParse(fromConfig, out var candidate);
    //     var lastSyncId = success ? candidate : 0;
    //     List<Project> unsyncedProjects = await _db.Projects
    //                                 .Where(project => project.Id > lastSyncId)
    //                                 .ToListAsync();
    //     return unsyncedProjects;
    // }
}
