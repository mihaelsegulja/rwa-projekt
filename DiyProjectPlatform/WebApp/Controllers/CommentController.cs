using AutoMapper;
using Core.Dtos;
using Core.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using WebApp.ViewModels;

namespace WebApp.Controllers
{
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
            dto.UserId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            dto.DateCreated = DateTime.UtcNow;

            await _commentService.AddCommentAsync(dto);
            return RedirectToAction("Details", "Project", new { id = dto.ProjectId });
        }

        [HttpPost]
        public async Task<IActionResult> Update(CommentUpdateVm vm)
        {
            int userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var dto = _mapper.Map<CommentUpdateDto>(vm);
            var result = await _commentService.UpdateCommentAsync(dto, userId);
            TempData["Message"] = result ?? "Update failed";
            return RedirectToAction("Details", "Project", new { id = vm.ProjectId });
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id, int projectId)
        {
            var result = await _commentService.DeleteCommentAsync(id);
            TempData["Message"] = result ?? "Delete failed";
            return RedirectToAction("Details", "Project", new { id = projectId });
        }
    }
}
