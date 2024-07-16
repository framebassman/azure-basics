using AutoMapper;

namespace ProjectTasks.WebApi.Models;

public class MapperProfile : Profile
{
    public MapperProfile()
    {
        CreateMap<DataAccess.AzureSQL.Project, ProjectResponse>();
        CreateMap<DataAccess.AzureSQL.Ticket, TicketResponse>();
        CreateMap<DataAccess.CosmosDb.Project, ProjectResponse>();
        CreateMap<DataAccess.CosmosDb.Ticket, TicketResponse>();
    }
}
