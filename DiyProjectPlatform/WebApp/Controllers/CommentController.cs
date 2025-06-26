using AutoMapper;
using Core.Dtos;
using Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shared.Exceptions;
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
    public async Task<IActionResult> Add(CommentAddVm vm)
    {
        if (!ModelState.IsValid)
        {
            TempData["Error"] = "Invalid comment";
            return RedirectToAction("Details", "Project", new { id = vm.ProjectId });
        }

        try
        {
            var dto = new CommentDto
            {
                ProjectId = vm.ProjectId,
                Content = vm.Content.Trim(),
                UserId = ClaimsHelper.GetClaimValueAsInt(User, ClaimTypes.NameIdentifier),
                DateCreated = DateTime.UtcNow
            };

            await _commentService.AddCommentAsync(dto);
            TempData["Success"] = "Comment added";
        }
        catch (AppException e)
        {
            TempData["Error"] = e.Message;
        }

        return RedirectToAction("Details", "Project", new { id = vm.ProjectId });
    }

    [HttpPost]
    public async Task<IActionResult> Update(CommentUpdateVm vm)
    {
        if (!ModelState.IsValid)
        {
            TempData["Error"] = "Invalid update";
            return RedirectToAction("Details", "Project", new { id = vm.ProjectId });
        }

        try
        {
            int userId = ClaimsHelper.GetClaimValueAsInt(User, ClaimTypes.NameIdentifier);
            var dto = _mapper.Map<CommentUpdateDto>(vm);
            var result = await _commentService.UpdateCommentAsync(dto, userId);
            TempData["Success"] = result;
        }
        catch (AppException e)
        {
            TempData["Error"] = e.Message;
        }
        
        return RedirectToAction("Details", "Project", new { id = vm.ProjectId });
    }

    [HttpPost]
    public async Task<IActionResult> Delete(int id, int projectId)
    {
        try
        {
            await _commentService.DeleteCommentAsync(id);
            TempData["Success"] = "Comment deleted";
        }
        catch (AppException e)
        {
            TempData["Error"] = e.Message;
        }

        return RedirectToAction("Details", "Project", new { id = projectId });
    }
}
