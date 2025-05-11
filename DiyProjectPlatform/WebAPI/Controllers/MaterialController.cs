using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebAPI.Dtos;
using WebAPI.Models;

namespace WebAPI.Controllers;

[Route("api/material")]
[ApiController]
[Authorize]
public class MaterialController : ControllerBase
{
    private readonly DbDiyProjectPlatformContext _dbContext;
    private readonly IMapper _mapper;

    public MaterialController(DbDiyProjectPlatformContext dbContext, IMapper mapper)
    {
        _dbContext = dbContext;
        _mapper = mapper;
    }
    
    [HttpGet("all")]
    public async Task<ActionResult<IEnumerable<MaterialDto>>> GetAllMaterials()
    {
        try
        {
            var materials = await _dbContext.Materials.ToListAsync();
            
            return Ok(_mapper.Map<IEnumerable<MaterialDto>>(materials));
        }
        catch (Exception e)
        {
            return StatusCode(500, e.Message);
        }
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<MaterialDto>> GetMaterialById(int id)
    {
        try
        {
            var material = await _dbContext.Materials.FindAsync(id);
            if (material == null) 
                return NotFound();
            
            return Ok(_mapper.Map<MaterialDto>(material));
        }
        catch (Exception e)
        {
            return StatusCode(500, e.Message);
        }
    }

    [HttpPost("add")]
    public async Task<IActionResult> AddMaterial(string material)
    {
        try
        {
            var trimmedMaterial = material.Trim();
            
            if(await _dbContext.Materials.AnyAsync(x => x.Name == trimmedMaterial))
                return BadRequest($"Material {trimmedMaterial} already exists");
            
            var newMaterial = new Material
            {
                Name = trimmedMaterial,
            };

            await _dbContext.Materials.AddAsync(_mapper.Map<Material>(newMaterial));
            await _dbContext.SaveChangesAsync();
            
            return Ok($"Material {trimmedMaterial} successfully added");
        }
        catch (Exception e)
        {
            return StatusCode(500, e.Message);
        }
    }

    [HttpPut("update")]
    public async Task<IActionResult> UpdateMaterial(MaterialDto material)
    {
        try
        {
            var existingMaterial = await _dbContext.Materials.FindAsync(material.Id);
            if (existingMaterial == null)
                return NotFound();

            existingMaterial.Name = material.Name.Trim();
            await _dbContext.SaveChangesAsync();

            return Ok($"Material {material.Id} successfully updated");
        }
        catch (Exception e)
        {
            return StatusCode(500, e.Message);
        }
    }
}
