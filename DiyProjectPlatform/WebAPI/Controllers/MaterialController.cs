using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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
            {
                return NotFound();
            }
            return Ok(material);
        }
        catch (Exception e)
        {
            return StatusCode(500, e.Message);
        }
    }

    [HttpPost("add")]
    public IActionResult AddMaterial([FromBody] Material material)
    {
        try
        {
            if (material == null)
            {
                return BadRequest("Material cannot be null");
            }
            _dbContext.Materials.Add(material);
            _dbContext.SaveChanges();
            //return CreatedAtAction(nameof(GetMaterialById), new { id = material.Id }, material);
            return Ok();
        }
        catch (Exception e)
        {
            return StatusCode(500, e.Message);
        }
    }
}
