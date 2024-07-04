using AutoMapper;

namespace ProjectTasks.Api.Models;

public class MapperProfile : Profile
{
    public MapperProfile()
    {
        CreateMap<Project, ProjectResponse>();
        CreateMap<Task, TaskResponse>();
        CreateMap<UnsyncronizedProject, ProjectResponse>();
        CreateMap<UnsyncronizedTask, TaskResponse>();
        CreateMap<ProjectRequest, UnsyncronizedProject>();
        CreateMap<TaskRequest, UnsyncronizedTask>();
        CreateMap<ProjectRequest, Project>();
        CreateMap<TaskRequest, Task>();
    }
}
