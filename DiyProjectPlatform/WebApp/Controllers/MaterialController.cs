using AutoMapper;
using Core.Dtos;
using Core.Interfaces;
using Core.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shared.Enums;
using Shared.Exceptions;
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
            TempData["Error"] = "Invalid material";
            var materials = await _materialService.GetAllMaterialsAsync();
            return View("Index", _mapper.Map<List<MaterialVm>>(materials));
        }

        try
        {
            var result = await _materialService.AddMaterialAsync(vm.Name.Trim());
            TempData["Success"] = result;
        }
        catch (AppException e)
        {
            TempData["Error"] = e.Message;
        }

        return RedirectToAction("Index");
    }

    [HttpPost]
    public async Task<IActionResult> Update(MaterialVm vm)
    {
        if (!ModelState.IsValid)
        {
            TempData["Error"] = "Invalid update";
            var materials = await _materialService.GetAllMaterialsAsync();
            return View("Index", _mapper.Map<List<MaterialVm>>(materials));
        }

        try
        {
            vm.Name = vm.Name.Trim();
            var dto = _mapper.Map<MaterialDto>(vm);
            var result = await _materialService.UpdateMaterialAsync(dto);
            TempData["Success"] = result;
        }
        catch (AppException e)
        {
            TempData["Error"] = e.Message;
        }

        return RedirectToAction("Index");
    }

    [HttpPost]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            var result = await _materialService.DeleteMaterialAsync(id);
            TempData["Success"] = result;
        }
        catch (AppException e)
        {
            TempData["Error"] = e.Message;
        }

        return RedirectToAction("Index");
    }
}
