using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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
    public IActionResult GetAllMaterials()
    {
        try
        {
            var materials = _dbContext.Materials.ToList();
            
            return Ok(_mapper.Map<IEnumerable<MaterialDto>>(materials));
        }
        catch (Exception e)
        {
            return StatusCode(500, e.Message);
        }
    }

    [HttpGet("{id}")]
    public IActionResult GetMaterialById(int id)
    {
        try
        {
            var material = _dbContext.Materials.Find(id);
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
    public IActionResult AddMaterial([FromBody] MaterialDto material)
    {
        try
        {
            var trimmedMaterial = material.Name.Trim();
            
            if(_dbContext.Materials.Any(x => x.Name == trimmedMaterial))
                return BadRequest($"Material {trimmedMaterial} already exists");
            
            material.Name = trimmedMaterial;
            _dbContext.Materials.Add(_mapper.Map<Material>(material));
            _dbContext.SaveChanges();
            
            return Ok($"Material {trimmedMaterial} successfully added");
        }
        catch (Exception e)
        {
            return StatusCode(500, e.Message);
        }
    }

    [HttpPut("update")]
    public IActionResult UpdateMaterial(int id, MaterialDto material)
    {
        try
        {
            var existingMaterial = _dbContext.Materials.Find(id);
            if (existingMaterial == null)
                return NotFound();

            existingMaterial.Name = material.Name.Trim();
            _dbContext.SaveChanges();

            return Ok($"Material {id} successfully updated");
        }
        catch (Exception e)
        {
            return StatusCode(500, e.Message);
        }
    }
}
