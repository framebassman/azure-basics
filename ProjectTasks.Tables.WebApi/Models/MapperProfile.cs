using AutoMapper;

namespace ProjectTasks.Tables.WebApi.Models;

public class MapperProfile : Profile
{
    public MapperProfile()
    {
        CreateMap<Project, ProjectResponse>();
        CreateMap<Task, TaskResponse>();
    }
}
