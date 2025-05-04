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

    // TODO: also add GetAllMaterials without pagination if needed

    [HttpGet("all")]
    public IActionResult GetAllMaterials(int page = 1, int pageSize = 10)
    {
        try
        {
            var materials = _dbContext.Materials
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();
            return Ok(materials);
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
            
            return Ok(material);
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
}
