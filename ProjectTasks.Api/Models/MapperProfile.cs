using AutoMapper;

namespace ProjectTasks.Api.Models;

public class MapperProfile : Profile
{
    public MapperProfile()
    {
        CreateMap<Project, ProjectResponse>();
        CreateMap<Task, TaskResponse>();
    }
}
