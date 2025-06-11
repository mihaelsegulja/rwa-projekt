using AutoMapper;
using Core.Dtos;
using Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shared.Helpers;
using System.Security.Claims;
using WebApp.ViewModels;

namespace WebApp.Controllers;

[Authorize]
public class CommentController : Controller
{
    private readonly ICommentService _commentService;
    private readonly IMapper _mapper;

    public CommentController(ICommentService commentService, IMapper mapper)
    {
        _commentService = commentService;
        _mapper = mapper;
    }

    [HttpPost]
    public async Task<IActionResult> Add(CommentDto dto)
    {
        if (!ModelState.IsValid)
        {
            return RedirectToAction("Details", "Project", new { id = dto.ProjectId });
        }

        dto.UserId = ClaimsHelper.GetClaimValueAsInt(User, ClaimTypes.NameIdentifier);
        dto.DateCreated = DateTime.UtcNow;

        await _commentService.AddCommentAsync(dto);
        TempData["Success"] = "Comment added";
        return RedirectToAction("Details", "Project", new { id = dto.ProjectId });
    }

    [HttpPost]
    public async Task<IActionResult> Update(CommentUpdateVm vm)
    {
        if (!ModelState.IsValid)
        {
            return RedirectToAction("Details", "Project", new { id = vm.ProjectId });
        }

        int userId = ClaimsHelper.GetClaimValueAsInt(User, ClaimTypes.NameIdentifier);
        var dto = _mapper.Map<CommentUpdateDto>(vm);
        var result = await _commentService.UpdateCommentAsync(dto, userId);
        if (result != null)
            TempData["Success"] = result;
        else
            TempData["Error"] = "Update failed";

        return RedirectToAction("Details", "Project", new { id = vm.ProjectId });
    }

    [HttpPost]
    public async Task<IActionResult> Delete(int id, int projectId)
    {
        var result = await _commentService.DeleteCommentAsync(id);
        if (result != null)
            TempData["Success"] = result;
        else
            TempData["Error"] = "Delete failed";

        return RedirectToAction("Details", "Project", new { id = projectId });
    }
}
