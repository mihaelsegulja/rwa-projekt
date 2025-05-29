using AutoMapper;
using Core.Models;
using Core.Dtos;

namespace Core.Mappings;

public class CoreMappingProfile : Profile
{
    public CoreMappingProfile()
    {
        CreateMap<Material, MaterialDto>().ReverseMap();
        CreateMap<Topic, TopicDto>().ReverseMap();
        CreateMap<User, UserRegisterDto>().ReverseMap();
        CreateMap<User, UserDto>().ReverseMap();
        CreateMap<User, UserProfileDto>().ReverseMap();
        CreateMap<Comment, CommentDto>().ReverseMap();
        CreateMap<Project, ProjectDto>().ReverseMap();
        CreateMap<ProjectStatus, ProjectStatusDto>().ReverseMap();
        CreateMap<Image, ImageDto>().ReverseMap();
        CreateMap<Log, LogDto>().ReverseMap();
    }
}