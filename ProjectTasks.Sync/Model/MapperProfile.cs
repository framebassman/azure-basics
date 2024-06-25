using AutoMapper;
using ProjectTasks.Sync.Model.CosmosDb;
using ProjectTasks.Sync.Model.Sql;

namespace ProjectTasks.Sync.Model
{
    public class MapperProfile : Profile
    {
        public MapperProfile()
        {
            CreateMap<UnsyncronizedProject, Project>();
            CreateMap<UnsyncronizedTask, CosmosDb.Task>();
        }
    }
}
