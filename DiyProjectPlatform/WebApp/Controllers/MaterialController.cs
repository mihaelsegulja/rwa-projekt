using AutoMapper;
using Core.Dtos;
using Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shared.Enums;
using WebApp.ViewModels;

namespace WebApp.Controllers;

[Authorize(Roles = nameof(UserRole.Admin))]
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
        return View(_mapper.Map<List<MaterialVm>>(materials));
    }

    [HttpPost]
    public async Task<IActionResult> Add(MaterialVm vm)
    {
        if (!ModelState.IsValid)
        {
            var materials = await _materialService.GetAllMaterialsAsync();
            return View("Index", _mapper.Map<List<MaterialVm>>(materials));
        }

        await _materialService.AddMaterialAsync(vm.Name.Trim());
        return RedirectToAction("Index");
    }

    [HttpPost]
    public async Task<IActionResult> Update(MaterialVm vm)
    {
        if (!ModelState.IsValid)
        {
            var materials = await _materialService.GetAllMaterialsAsync();
            return View("Index", _mapper.Map<List<MaterialVm>>(materials));
        }

        vm.Name = vm.Name.Trim();
        var dto = _mapper.Map<MaterialDto>(vm);
        var result = await _materialService.UpdateMaterialAsync(dto);
        if (result != null)
            TempData["Success"] = result;
        else
            TempData["Error"] = "Update failed";

        return RedirectToAction("Index");
    }
}
