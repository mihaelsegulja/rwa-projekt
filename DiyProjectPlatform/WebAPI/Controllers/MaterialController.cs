using AutoMapper;
using Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Core.Dtos;

namespace WebAPI.Controllers;

[Route("api/material")]
[ApiController]
[Authorize]
public class MaterialController : ControllerBase
{
    private readonly IMaterialService _materialService;
    private readonly IMapper _mapper;

    public MaterialController(IMaterialService materialService, IMapper mapper)
    {
        _materialService = materialService;
        _mapper = mapper;
    }
    
    [HttpGet("all")]
    public async Task<IActionResult> GetAllMaterials()
    {
        try
        {
            var materials = await _materialService.GetAllMaterialsAsync();
            return Ok(materials);
        }
        catch (Exception e)
        {
            return StatusCode(500, e.Message);
        }
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetMaterialById(int id)
    {
        try
        {
            var material = await _materialService.GetMaterialByIdAsync(id);
            return material == null ? NotFound() : Ok(material);
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
            var result = await _materialService.AddMaterialAsync(material);
            return Ok(result);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
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
            var result = await _materialService.UpdateMaterialAsync(material);
            return result == null ? NotFound() : Ok(result);
        }
        catch (Exception e)
        {
            return StatusCode(500, e.Message);
        }
    }

    [HttpDelete("delete")]
    public async Task<IActionResult> DeleteMaterial(int id)
    {
        try
        {
            var result = await _materialService.DeleteMaterialAsync(id);
            return result == null ? NotFound() : Ok(result);
        }
        catch (Exception e)
        {
            return StatusCode(500, e.Message);
        }
    }
}
