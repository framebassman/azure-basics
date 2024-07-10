using AutoMapper;
using ProjectTasks.DataAccess.AzureSQL;

namespace ProjectTasks.WebApi.Models;

public class MapperProfile : Profile
{
    public MapperProfile()
    {
        CreateMap<Project, ProjectResponse>();
        CreateMap<Ticket, TicketResponse>();
    }
}
