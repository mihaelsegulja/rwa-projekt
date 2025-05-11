using AutoMapper;
using WebAPI.Dtos;
using WebAPI.Models;

namespace WebAPI.Mappings;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<Log, LogDto>().ReverseMap();
        CreateMap<User, UserRegisterDto>().ReverseMap();
        CreateMap<User, UserDto>().ReverseMap();
        CreateMap<User, UserProfileDto>().ReverseMap();
        CreateMap<Comment, CommentDto>().ReverseMap();
        CreateMap<Material, MaterialDto>().ReverseMap();
        CreateMap<Topic, TopicDto>().ReverseMap();
        CreateMap<Project, ProjectDto>().ReverseMap();
        CreateMap<ProjectStatus, ProjectStatusDto>().ReverseMap();
        CreateMap<Image, ImageDto>().ReverseMap();
        CreateMap<ProjectDetailDto, Project>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.Project.Title))
            .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Project.Description))
            .ForMember(dest => dest.Content, opt => opt.MapFrom(src => src.Project.Content))
            .ForMember(dest => dest.TopicId, opt => opt.MapFrom(src => src.Project.TopicId))
            .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.Project.UserId))
            .ForMember(dest => dest.DifficultyLevelId, opt => opt.MapFrom(src => src.Project.DifficultyLevelId))
            .ForMember(dest => dest.DateCreated, opt => opt.Ignore())
            .ForMember(dest => dest.DateModified, opt => opt.Ignore())
            .ForMember(dest => dest.ProjectMaterials, opt => opt.Ignore())
            .ForMember(dest => dest.ProjectImages, opt => opt.Ignore())
            .ForMember(dest => dest.ProjectStatuses, opt => opt.Ignore());
        CreateMap<Project, ProjectDetailDto>()
            .ForMember(dest => dest.Project, opt => opt.MapFrom(src => src))
            .ForMember(dest => dest.MaterialIds, opt => opt.MapFrom(src => src.ProjectMaterials.Select(pm => pm.MaterialId)))
            .ForMember(dest => dest.Images, opt => opt.MapFrom(src => src.ProjectImages.Select(pi => new ImageDto
            {
                Id = pi.Image.Id,
                Description = pi.Image.Description,
                ImageData = pi.Image.ImageData,
                IsMainImage = pi.IsMainImage
            })));

    }
}