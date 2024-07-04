using AutoMapper;
using ProjectTasks.DataAccess.Common;

namespace ProjectTasks.Tables.WebApi.Models;

public class MapperProfile : Profile
{
    public MapperProfile()
    {
        CreateMap<Project, ProjectResponse>();
        CreateMap<Ticket, TicketResponse>();
    }
}
