using AutoMapper;
using Core.Dtos;
using Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

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
        var materials = await _materialService.GetAllMaterialsAsync();
        return Ok(materials);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetMaterialById(int id)
    {
        var material = await _materialService.GetMaterialByIdAsync(id);
        return Ok(material);
    }

    [HttpPost("add")]
    public async Task<IActionResult> AddMaterial(string material)
    {
        var result = await _materialService.AddMaterialAsync(material);
        return Ok(result);
    }

    [HttpPut("update")]
    public async Task<IActionResult> UpdateMaterial(MaterialDto material)
    {
        var result = await _materialService.UpdateMaterialAsync(material);
        return Ok(result);
    }

    [HttpDelete("delete")]
    public async Task<IActionResult> DeleteMaterial(int id)
    {
        var result = await _materialService.DeleteMaterialAsync(id);
        return Ok(result);
    }
}
