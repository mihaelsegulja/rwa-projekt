using AutoMapper;
using Core.Dtos;
using Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shared.Enums;
using WebApp.ViewModels;

namespace WebApp.Controllers;

[Authorize(Roles = nameof(UserRole.Admin))]
public class TopicController : Controller
{
    private readonly ITopicService _topicService;
    private readonly IMapper _mapper;

    public TopicController(ITopicService topicService, IMapper mapper)
    {
        _topicService = topicService;
        _mapper = mapper;
    }

    public async Task<IActionResult> Index()
    {
        var topics = await _topicService.GetAllTopicsAsync();
        return View(_mapper.Map<List<TopicVm>>(topics));
    }

    [HttpPost]
    public async Task<IActionResult> Add(TopicVm vm)
    {
        if (!ModelState.IsValid)
        {
            var topics = await _topicService.GetAllTopicsAsync();
            return View("Index", _mapper.Map<List<TopicVm>>(topics));
        }

        var result = await _topicService.AddTopicAsync(vm.Name.Trim());
        if (result != null)
            TempData["Success"] = result;
        else
            TempData["Error"] = "Failed to add new topic";

        return RedirectToAction("Index");
    }

    [HttpPost]
    public async Task<IActionResult> Update(TopicVm vm)
    {
        if (!ModelState.IsValid)
        {
            var topics = await _topicService.GetAllTopicsAsync();
            return View("Index", _mapper.Map<List<TopicVm>>(topics));
        }

        vm.Name = vm.Name.Trim();
        var dto = _mapper.Map<TopicDto>(vm);
        var result = await _topicService.UpdateTopicAsync(dto);
        if (result != null)
            TempData["Success"] = result;
        else
            TempData["Error"] = "Update failed";

        return RedirectToAction("Index");
    }
}
