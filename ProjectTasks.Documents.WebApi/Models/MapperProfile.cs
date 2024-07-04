using AutoMapper;
using ProjectTasks.DataAccess.CosmosDb;

namespace ProjectTasks.Documents.WebApi.Models;

public class MapperProfile : Profile
{
    public MapperProfile()
    {
        CreateMap<Project, ProjectResponse>();
        CreateMap<Ticket, TicketResponse>();
    }
}
