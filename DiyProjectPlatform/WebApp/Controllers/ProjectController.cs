using AutoMapper;
using Core.Dtos;
using Core.Interfaces;
using Core.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Shared.Enums;
using Shared.Helpers;
using System.Security.Claims;
using System.Text.Json;
using WebApp.ViewModels;

namespace WebApp.Controllers;

[Authorize]
public class ProjectController : Controller
{
    private readonly IProjectService _projectService;
    private readonly ITopicService _topicService;
    private readonly IMaterialService _materialService;
    private readonly IMapper _mapper;

    public ProjectController(IProjectService projectService, IMapper mapper, ITopicService topicService, IMaterialService materialService)
    {
        _projectService = projectService;
        _mapper = mapper;
        _topicService = topicService;
        _materialService = materialService;
    }

    public async Task<IActionResult> Index(int page = 1, int pageSize = 10)
    {
        var userRole = ClaimsHelper.GetClaimValue(User, ClaimTypes.Role);
        var projects = await _projectService.GetAllProjectsAsync(userRole, page, pageSize);
        var vms = _mapper.Map<List<ProjectListVm>>(projects);
        return View(vms);
    }

    public async Task<IActionResult> Details(int id)
    {
        var dto = await _projectService.GetProjectByIdAsync(id);
        if (dto == null)
            return NotFound();

        var vm = _mapper.Map<ProjectDetailVm>(dto);
        return View(vm);
    }

    [Authorize]
    [HttpGet]
    public async Task<IActionResult> Create()
    {
        var topics = await _topicService.GetAllTopicsAsync();
        var materials = await _materialService.GetAllMaterialsAsync();
        var difficultyLevels = Enum.GetValues(typeof(Shared.Enums.DifficultyLevel))
            .Cast<Shared.Enums.DifficultyLevel>()
            .Select(d => new SelectListItem
            {
                Value = ((int)d).ToString(),
                Text = d.ToString()
            })
            .ToList();

        var model = new ProjectCreateVm
        {
            Topics = topics.Select(t => new SelectListItem { Value = t.Id.ToString(), Text = t.Name }).ToList(),
            DifficultyLevels = difficultyLevels,
            AllMaterials = materials.ToList()
        };

        return View(model);
    }

    [Authorize]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(ProjectCreateVm vm, string imagesJson)
    {
        if (!ModelState.IsValid)
        {
            // repopulate dropdowns if validation fails
            vm.Topics = (await _topicService.GetAllTopicsAsync())
                .Select(t => new SelectListItem { Value = t.Id.ToString(), Text = t.Name }).ToList();
            vm.AllMaterials = (await _materialService.GetAllMaterialsAsync()).ToList();
            vm.DifficultyLevels = Enum.GetValues(typeof(Shared.Enums.DifficultyLevel))
            .Cast<Shared.Enums.DifficultyLevel>()
            .Select(d => new SelectListItem
            {
                Value = ((int)d).ToString(),
                Text = d.ToString()
            })
            .ToList();

            return View(vm);
        }

        var currentUserId = ClaimsHelper.GetClaimValueAsInt(User, ClaimTypes.NameIdentifier);

        var images = JsonSerializer.Deserialize<List<ImageDto>>(imagesJson);

        var createDto = new ProjectCreateDto
        {
            Project = vm.Project,
            MaterialIds = vm.SelectedMaterialIds,
            Images = images ?? new()
        };

        var result = await _projectService.AddProjectAsync(createDto, currentUserId);

        TempData["SuccessMessage"] = result;
        return RedirectToAction("Index");
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var projectDetail = await _projectService.GetProjectByIdAsync(id);
        if (projectDetail == null)
            return NotFound();

        var materials = await _materialService.GetAllMaterialsAsync();
        var topics = await _topicService.GetAllTopicsAsync();

        var vm = new ProjectEditVm
        {
            Project = projectDetail.Project,
            AllMaterials = materials.Select(m => new SelectListItem { Value = m.Id.ToString(), Text = m.Name }).ToList(),
            SelectedMaterialIds = projectDetail.Materials.Select(m => m.Id).ToList(),
            Topics = topics.Select(t => new SelectListItem { Value = t.Id.ToString(), Text = t.Name }).ToList(),
            DifficultyLevels = Enum.GetValues<Shared.Enums.DifficultyLevel>().Select(dl => new SelectListItem
            {
                Value = ((int)dl).ToString(),
                Text = dl.ToString()
            }).ToList()
        };

        return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(ProjectEditVm vm)
    {
        if (!ModelState.IsValid)
        {
            // repopulate selects in case of error
            var materials = await _materialService.GetAllMaterialsAsync();
            var topics = await _topicService.GetAllTopicsAsync();
            vm.AllMaterials = materials.Select(m => new SelectListItem { Value = m.Id.ToString(), Text = m.Name }).ToList();
            vm.Topics = topics.Select(t => new SelectListItem { Value = t.Id.ToString(), Text = t.Name }).ToList();
            vm.DifficultyLevels = Enum.GetValues<Shared.Enums.DifficultyLevel>().Select(dl => new SelectListItem
            {
                Value = ((int)dl).ToString(),
                Text = dl.ToString()
            }).ToList();
            return View(vm);
        }

        var currentUserId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        var updateDto = new ProjectUpdateDto
        {
            Project = vm.Project,
            MaterialIds = vm.SelectedMaterialIds
        };

        var result = await _projectService.UpdateProjectAsync(updateDto, currentUserId);
        if (result == null)
            return NotFound();

        TempData["Success"] = result;
        return RedirectToAction("Index");
    }



    [Authorize(Roles = nameof(Shared.Enums.UserRole.Admin))]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var userId = ClaimsHelper.GetClaimValueAsInt(User, ClaimTypes.NameIdentifier);
        var result = await _projectService.DeleteProjectAsync(id, userId);
        if (result == null) return NotFound();

        return RedirectToAction("Index");
    }
}
