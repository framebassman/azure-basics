using AutoMapper;

namespace ProjectTasks.CosmosDb.Models;

public class MapperProfile : Profile
{
    public MapperProfile()
    {
        CreateMap<Project, ProjectResponse>();
        CreateMap<Task, TaskResponse>();
    }
}
