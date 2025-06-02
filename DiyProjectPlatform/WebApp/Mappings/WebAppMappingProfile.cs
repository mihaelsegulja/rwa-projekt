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
    }
}
