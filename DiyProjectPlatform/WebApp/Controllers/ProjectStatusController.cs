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

    public ProjectStatusController(IProjectService projectService)
    {
        _projectService = projectService;
    }

    public async Task<IActionResult> Index()
    {
        var statuses = await _projectService.GetAllProjectStatusesAsync(1, 100);

        var vm = statuses.Select(s => new ProjectStatusApprovalVm
        {
            Id = s.Id,
            ProjectId = s.ProjectId,
            ProjectTitle = s.ProjectTitle,
            AuthorUsername = s.AuthorUsername,
            StatusTypeId = s.StatusTypeId,
            DateModified = s.DateModified,
            ApproverUsername = s.ApproverUsername,
            SelectedStatusTypeId = s.StatusTypeId,
            StatusTypeOptions = Enum.GetValues(typeof(ProjectStatusType))
                .Cast<ProjectStatusType>()
                .Select(st => new SelectListItem
                {
                    Text = st.ToString(),
                    Value = ((int)st).ToString()
                }).ToList()
        }).ToList();

        return View(vm);
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
        if (result != null)
            TempData["Success"] = result;
        else
            TempData["Error"] = "Failed to update project status.";

        return RedirectToAction("Index");
    }
}
