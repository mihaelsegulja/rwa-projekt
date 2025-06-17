using AutoMapper;
using Core.Context;
using Core.Dtos;
using Core.Interfaces;
using Core.Models;
using Microsoft.EntityFrameworkCore;
using Shared.Enums;
using Shared.Exceptions;

namespace Core.Services;

public class MaterialService : IMaterialService
{
    private readonly DbDiyProjectPlatformContext _dbContext;
    private readonly IMapper _mapper;
    private readonly ILogService _logService;

    public MaterialService(DbDiyProjectPlatformContext context, IMapper mapper, ILogService logService)
    {
        _dbContext = context;
        _mapper = mapper;
        _logService = logService;
    }

    public async Task<IEnumerable<MaterialDto>> GetAllMaterialsAsync()
    {
        var materials = await _dbContext.Materials.ToListAsync();
        return _mapper.Map<IEnumerable<MaterialDto>>(materials);
    }

    public async Task<MaterialDto> GetMaterialByIdAsync(int id)
    {
        var material = await _dbContext.Materials.FindAsync(id) 
            ?? throw new NotFoundException($"Material {id} not found");

        return _mapper.Map<MaterialDto>(material);
    }

    public async Task<string> AddMaterialAsync(string name)
    {
        var trimmed = name.Trim();

        if (await _dbContext.Materials.AnyAsync(m => m.Name == trimmed))
            throw new ConflictException($"Material '{trimmed}' already exists");

        var material = new Material { Name = trimmed };
        await _dbContext.Materials.AddAsync(material);
        await _dbContext.SaveChangesAsync();
        await _logService.AddLogAsync($"Material '{trimmed}' added", LogLevel.Info);

        return $"Material {trimmed} successfully added";
    }

    public async Task<string> UpdateMaterialAsync(MaterialDto materialDto)
    {
        var material = await _dbContext.Materials.FindAsync(materialDto.Id) 
            ?? throw new NotFoundException($"Material {materialDto.Id} not found");

        material.Name = materialDto.Name.Trim();
        await _dbContext.SaveChangesAsync();
        await _logService.AddLogAsync($"Material {material.Id} updated", LogLevel.Info);

        return $"Material {material.Id} successfully updated";
    }

    public async Task<string> DeleteMaterialAsync(int id)
    {
        var material = await _dbContext.Materials.FindAsync(id) 
            ?? throw new NotFoundException($"Material {id} not found");

        _dbContext.Materials.Remove(material);
        await _dbContext.SaveChangesAsync();
        await _logService.AddLogAsync($"Material {id} deleted", LogLevel.Info);

        return $"Material {id} successfully deleted";
    }
}
