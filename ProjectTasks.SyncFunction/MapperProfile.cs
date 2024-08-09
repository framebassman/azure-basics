using AutoMapper;

namespace ProjectTasks.SyncFunction;

public class MapperProfile : Profile
{
    public MapperProfile()
    {
        CreateMap<DataAccess.AzureSQL.Project, DataAccess.CosmosDb.Project>();
        CreateMap<DataAccess.AzureSQL.Ticket, DataAccess.CosmosDb.Ticket>();
    }
}
