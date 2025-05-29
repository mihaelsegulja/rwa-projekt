using AutoMapper;
using Core.Context;
using Core.Interfaces;
using Core.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Dtos;

namespace Core.Services;

public class MaterialService : IMaterialService
{
    private readonly DbDiyProjectPlatformContext _dbContext;
    private readonly IMapper _mapper;

    public MaterialService(DbDiyProjectPlatformContext context, IMapper mapper)
    {
        _dbContext = context;
        _mapper = mapper;
    }

    public async Task<IEnumerable<MaterialDto>> GetAllMaterialsAsync()
    {
        var materials = await _dbContext.Materials.ToListAsync();
        return _mapper.Map<IEnumerable<MaterialDto>>(materials);
    }

    public async Task<MaterialDto?> GetMaterialByIdAsync(int id)
    {
        var material = await _dbContext.Materials.FindAsync(id);
        return material == null ? null : _mapper.Map<MaterialDto>(material);
    }

    public async Task<string> AddMaterialAsync(string name)
    {
        var trimmed = name.Trim();

        if (await _dbContext.Materials.AnyAsync(m => m.Name == trimmed))
            throw new InvalidOperationException($"Material '{trimmed}' already exists");

        var material = new Material { Name = trimmed };
        await _dbContext.Materials.AddAsync(material);
        await _dbContext.SaveChangesAsync();

        return $"Material '{trimmed}' successfully added";
    }

    public async Task<string?> UpdateMaterialAsync(MaterialDto materialDto)
    {
        var material = await _dbContext.Materials.FindAsync(materialDto.Id);
        if (material == null) return null;

        material.Name = materialDto.Name.Trim();
        await _dbContext.SaveChangesAsync();
        return $"Material {materialDto.Id} successfully updated";
    }
}
