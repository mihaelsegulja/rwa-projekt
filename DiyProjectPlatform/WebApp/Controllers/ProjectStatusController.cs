﻿using AutoMapper;
using Core.Dtos;
using Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Shared.Enums;
using Shared.Helpers;
using System.Security.Claims;
using WebApp.ViewModels;

namespace WebApp.Controllers;

[Authorize(Roles = nameof(UserRole.Admin))]
public class ProjectStatusController : Controller
{
    private readonly IProjectService _projectService;
    private readonly IMapper _mapper;

    public ProjectStatusController(IProjectService projectService, IMapper mapper)
    {
        _projectService = projectService;
        _mapper = mapper;
    }

    public async Task<IActionResult> Index(int page = 1, int pageSize = 10)
    {
        var result = await _projectService.GetAllProjectStatusesAsync(page, pageSize);

        var vm = new ProjectStatusFilterVm
        {
            Page = result.Page,
            PageSize = result.PageSize,
            TotalItems = result.TotalItems,
            StatusTypeOptions = Enum.GetValues(typeof(ProjectStatusType))
            .Cast<ProjectStatusType>()
            .Select(st => new SelectListItem
            {
                Text = st.ToString(),
                Value = ((int)st).ToString()
            }).ToList(),
            Statuses = _mapper.Map<List<ProjectStatusListVm>>(result.Items)
        };

        return View(vm);
    }
    public async Task<IActionResult> Search(int page = 1, int pageSize = 10)
    {
        var result = await _projectService.GetAllProjectStatusesAsync(page, pageSize);

        var vm = new ProjectStatusFilterVm
        {
            Page = result.Page,
            PageSize = result.PageSize,
            TotalItems = result.TotalItems,
            StatusTypeOptions = Enum.GetValues(typeof(ProjectStatusType))
            .Cast<ProjectStatusType>()
            .Select(st => new SelectListItem
            {
                Text = st.ToString(),
                Value = ((int)st).ToString()
            }).ToList(),
            Statuses = _mapper.Map<List<ProjectStatusListVm>>(result.Items)
        };

        return PartialView("_ProjectStatusListPartial", vm);
    }

    [HttpPost]
    public async Task<IActionResult> Update(int id, int projectId, int selectedStatusTypeId)
    {
        var approverId = ClaimsHelper.GetClaimValueAsInt(User, ClaimTypes.NameIdentifier);

        var dto = new ProjectStatusDto
        {
            Id = id,
            ProjectId = projectId,
            StatusTypeId = selectedStatusTypeId,
            DateModified = DateTime.UtcNow,
            ApproverId = approverId
        };

        var result = await _projectService.UpdateProjectStatusAsync(dto);
        TempData["Success"] = result;

        return RedirectToAction("Index");
    }
}
