using Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shared.Helpers;
using System.Security.Claims;

namespace WebApp.Controllers;

[Authorize]
public class ProjectController : Controller
{
    private readonly IProjectService _projectService;

    public ProjectController(IProjectService projectService)
    {
        _projectService = projectService;
    }

    public async Task<IActionResult> Index(int page = 1, int pageSize = 10)
    {
        var userRole = ClaimsHelper.GetClaimValue(User, ClaimTypes.Role);
        var projects = await _projectService.GetAllProjectsAsync(userRole, page, pageSize);
        return View(projects);
    }

    public async Task<IActionResult> Details(int id)
    {
        var project = await _projectService.GetProjectByIdAsync(id);
        if (project == null)
            return NotFound();

        return View(project);
    }
}
