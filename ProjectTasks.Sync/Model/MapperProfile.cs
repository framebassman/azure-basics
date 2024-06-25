using AutoMapper;

namespace ProjectTasks.Sync.Model
{
    public class MapperProfile : Profile
    {
        public MapperProfile()
        {
            CreateMap<Sql.UnsyncronizedProject, CosmosDb.Project>();
            CreateMap<Sql.UnsyncronizedTask, CosmosDb.Task>();
            CreateMap<Sql.UnsyncronizedProject, Sql.Project>()
                .ForMember(x => x.Id, opt => opt.Ignore());
            CreateMap<Sql.UnsyncronizedTask, Sql.Task>()
                .ForMember(x => x.Id, opt => opt.Ignore());
        }
    }
}
