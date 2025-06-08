using AutoMapper;
using Core.Dtos;
using Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApp.ViewModels;

namespace WebApp.Controllers;

[Authorize(Roles = nameof(Shared.Enums.UserRole.Admin))]
public class MaterialController : Controller
{
    private readonly IMaterialService _materialService;
    private readonly IMapper _mapper;

    public MaterialController(IMaterialService materialService, IMapper mapper)
    {
        _materialService = materialService;
        _mapper = mapper;
    }

    public async Task<IActionResult> Index()
    {
        var materials = await _materialService.GetAllMaterialsAsync();
        return View(materials);
    }

    [HttpPost]
    public async Task<IActionResult> Add(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            return BadRequest("Name is required"); //TODO: Change this

        await _materialService.AddMaterialAsync(name.Trim());
        return RedirectToAction("Index");
    }

    [HttpPost]
    public async Task<IActionResult> Update(MaterialVm vm)
    {
        vm.Name = vm.Name.Trim();
        var dto = _mapper.Map<MaterialDto>(vm);
        await _materialService.UpdateMaterialAsync(dto);
        return RedirectToAction("Index");
    }
}
