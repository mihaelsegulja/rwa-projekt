using AutoMapper;
using WebAPI.Dtos;
using WebAPI.Models;

namespace WebAPI.Mappings;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<Log, LogDto>();
    }
}