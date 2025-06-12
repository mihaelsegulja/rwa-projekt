using AutoMapper;
using Core.Dtos;
using Core.Interfaces;
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
    private readonly ICommentService _commentService;
    private readonly IMapper _mapper;

    public ProjectController(IProjectService projectService, IMapper mapper, ITopicService topicService, IMaterialService materialService, ICommentService commentService)
    {
        _projectService = projectService;
        _mapper = mapper;
        _topicService = topicService;
        _materialService = materialService;
        _commentService = commentService;
    }

    public async Task<IActionResult> Index(ProjectFilterVm filter)
    {
        var userRole = ClaimsHelper.GetClaimValue(User, ClaimTypes.Role);
        var dto = _mapper.Map<ProjectFilterDto>(filter);
        var result = await _projectService.GetAllProjectsAsync(userRole, dto);

        var vm = new ProjectFilterVm
        {
            Projects = _mapper.Map<List<ProjectListVm>>(result.Items),
            Page = result.Page,
            PageSize = result.PageSize,
            TotalItems = result.TotalItems,
            Search = filter.Search,
            TopicId = filter.TopicId,
            DifficultyLevelId = filter.DifficultyLevelId
        };

        vm.Projects = _mapper.Map<List<ProjectListVm>>(result.Items);

        var topics = await _topicService.GetAllTopicsAsync();
        vm.Topics = topics.Select(t => new SelectListItem { Value = t.Id.ToString(), Text = t.Name }).ToList();

        vm.DifficultyLevels = Enum.GetValues<DifficultyLevel>().Select(dl => new SelectListItem
        {
            Value = ((int)dl).ToString(),
            Text = dl.ToString()
        }).ToList();

        return View(vm);
    }

    [HttpGet]
    public async Task<IActionResult> Search(ProjectFilterVm filter)
    {
        var userRole = ClaimsHelper.GetClaimValue(User, ClaimTypes.Role);
        var dto = _mapper.Map<ProjectFilterDto>(filter);
        var result = await _projectService.GetAllProjectsAsync(userRole, dto);

        var vm = new ProjectFilterVm
        {
            Projects = _mapper.Map<List<ProjectListVm>>(result.Items),
            Page = result.Page,
            PageSize = result.PageSize,
            TotalItems = result.TotalItems,
            Search = filter.Search,
            TopicId = filter.TopicId,
            DifficultyLevelId = filter.DifficultyLevelId
        };

        var topics = await _topicService.GetAllTopicsAsync();
        vm.Topics = topics.Select(t => new SelectListItem { Value = t.Id.ToString(), Text = t.Name }).ToList();

        vm.DifficultyLevels = Enum.GetValues<DifficultyLevel>().Select(dl => new SelectListItem
        {
            Value = ((int)dl).ToString(),
            Text = dl.ToString()
        }).ToList();

        return PartialView("_ProjectListPartial", vm);
    }

    public async Task<IActionResult> Details(int id)
    {
        var dto = await _projectService.GetProjectByIdAsync(id);
        if (dto == null)
            return NotFound();

        var commentDtos = await _commentService.GetAllCommentsByProjectIdAsync(id, 1, 50);

        var vm = _mapper.Map<ProjectDetailVm>(dto);

        vm.Comments = commentDtos.Select(c => new CommentVm
        {
            Id = c.Id,
            Content = c.Content,
            DateCreated = c.DateCreated,
            Username = c.Username
        }).ToList();

        return View(vm);
    }

    [HttpGet]
    public async Task<IActionResult> Create()
    {
        var topics = await _topicService.GetAllTopicsAsync();
        var materials = await _materialService.GetAllMaterialsAsync();
        var difficultyLevels = Enum.GetValues(typeof(DifficultyLevel))
            .Cast<DifficultyLevel>()
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

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(ProjectCreateVm vm, string imagesJson)
    {
        if (!ModelState.IsValid)
        {
            vm.Topics = (await _topicService.GetAllTopicsAsync())
                .Select(t => new SelectListItem { Value = t.Id.ToString(), Text = t.Name }).ToList();
            vm.AllMaterials = (await _materialService.GetAllMaterialsAsync()).ToList();
            vm.DifficultyLevels = Enum.GetValues(typeof(DifficultyLevel))
            .Cast<DifficultyLevel>()
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
            Project = _mapper.Map<ProjectDto>(vm.Project),
            MaterialIds = vm.SelectedMaterialIds,
            Images = images ?? new()
        };

        var result = await _projectService.AddProjectAsync(createDto, currentUserId);

        TempData["Success"] = result;
        return RedirectToAction("Index");
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var projectDetail = await _projectService.GetProjectByIdAsync(id);
        if (projectDetail == null)
            return NotFound();

        if (User.Identity?.Name != projectDetail.Username && !User.IsInRole(nameof(UserRole.Admin)))
            return Forbid();

        var materials = await _materialService.GetAllMaterialsAsync();
        var topics = await _topicService.GetAllTopicsAsync();

        var vm = new ProjectEditVm
        {
            Project = _mapper.Map<ProjectVm>(projectDetail.Project),
            AllMaterials = materials.Select(m => new SelectListItem { Value = m.Id.ToString(), Text = m.Name }).ToList(),
            SelectedMaterialIds = projectDetail.Materials.Select(m => m.Id).ToList(),
            Topics = topics.Select(t => new SelectListItem { Value = t.Id.ToString(), Text = t.Name }).ToList(),
            DifficultyLevels = Enum.GetValues<DifficultyLevel>().Select(dl => new SelectListItem
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
            var materials = await _materialService.GetAllMaterialsAsync();
            var topics = await _topicService.GetAllTopicsAsync();
            vm.AllMaterials = materials.Select(m => new SelectListItem { Value = m.Id.ToString(), Text = m.Name }).ToList();
            vm.Topics = topics.Select(t => new SelectListItem { Value = t.Id.ToString(), Text = t.Name }).ToList();
            vm.DifficultyLevels = Enum.GetValues<DifficultyLevel>().Select(dl => new SelectListItem
            {
                Value = ((int)dl).ToString(),
                Text = dl.ToString()
            }).ToList();
            return View(vm);
        }

        var currentUserId = ClaimsHelper.GetClaimValueAsInt(User, ClaimTypes.NameIdentifier);

        var updateDto = new ProjectUpdateDto
        {
            Project = _mapper.Map<ProjectDto>(vm.Project),
            MaterialIds = vm.SelectedMaterialIds
        };

        var result = await _projectService.UpdateProjectAsync(updateDto, currentUserId);
        if (result == null)
            return NotFound();

        TempData["Success"] = result;
        return RedirectToAction("Index");
    }

    [Authorize(Roles = nameof(UserRole.Admin))]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var userId = ClaimsHelper.GetClaimValueAsInt(User, ClaimTypes.NameIdentifier);
        var result = await _projectService.DeleteProjectAsync(id, userId);
        if (result != null)
        {
            TempData["Success"] = result;
        }
        else
        {
            TempData["Error"] = "Update failed";
            return NotFound();
        }

        return RedirectToAction("Index");
    }
}
