using AutoMapper;
using Core.Dtos;
using WebApp.ViewModels;

namespace WebApp.Mappings;

public class WebAppMappingProfile : Profile
{
    public WebAppMappingProfile()
    {
        CreateMap<ProjectListDto, ProjectListVm>()
            .ForMember(dest => dest.Author, opt => opt.MapFrom(src => src.Username))
            .ForMember(dest => dest.DateCreated, opt => opt.MapFrom(src => src.DateCreated.ToString("dd.MM.yyyy.")));
        CreateMap<ChangePasswordDto, ChangePasswordVm>().ReverseMap();
        CreateMap<UserProfileDto, UserProfileVm>().ReverseMap();
        CreateMap<UserDto, UserProfileVm>();
        CreateMap<ProjectDetailDto, ProjectDetailVm>()
            .ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.Project.Title))
            .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Project.Description))
            .ForMember(dest => dest.Content, opt => opt.MapFrom(src => src.Project.Content))
            .ForMember(dest => dest.Author, opt => opt.MapFrom(src => src.Username))
            .ForMember(dest => dest.TopicName, opt => opt.MapFrom(src => src.TopicName))
            .ForMember(dest => dest.DifficultyLevelName, opt => opt.MapFrom(src => src.DifficultyLevelName))
            .ForMember(dest => dest.MaterialNames, opt => opt.MapFrom(src => src.Materials.Select(m => m.Name)))
            .ForMember(dest => dest.Images, opt => opt.MapFrom(src => src.Images));
        CreateMap<ImageDto, ImageVm>();
    }
}
