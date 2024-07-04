using AutoMapper;

namespace ProjectTasks.Documents.WebApi.Models;

public class MapperProfile : Profile
{
    public MapperProfile()
    {
        CreateMap<Project, ProjectResponse>();
        CreateMap<Task, TaskResponse>();
    }
}
