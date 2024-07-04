using AutoMapper;

namespace ProjectTasks.DataAccess.Common;

public class MapperProfile : Profile
{
    public MapperProfile()
    {
        CreateMap<Project, ProjectResponse>();
        CreateMap<Ticket, TicketResponse>();
    }
}
