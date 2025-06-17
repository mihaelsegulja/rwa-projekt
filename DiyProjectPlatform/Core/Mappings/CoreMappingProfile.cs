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
        CreateMap<Comment, CommentDto>()
            .ForMember(dest => dest.Username, opt => opt.MapFrom(src => src.User.Username));
        CreateMap<Project, ProjectDto>().ReverseMap();
        CreateMap<ProjectDto, Project>()
            .ForMember(dest => dest.UserId, opt => opt.Ignore())
            .ForMember(dest => dest.DateCreated, opt => opt.Ignore());
        CreateMap<ProjectStatus, ProjectStatusDto>().ReverseMap();
        CreateMap<Image, ImageDto>().ReverseMap();
        CreateMap<Log, LogDto>().ReverseMap();
        CreateMap<ProjectStatus, ProjectStatusListDto>()
            .ForMember(dest => dest.ProjectTitle, opt => opt.MapFrom(src => src.Project.Title))
            .ForMember(dest => dest.AuthorUsername, opt => opt.MapFrom(src => src.Project.User.Username))
            .ForMember(dest => dest.ApproverUsername, opt => opt.MapFrom(src => src.Approver != null ? src.Approver.Username : string.Empty));
    }
}