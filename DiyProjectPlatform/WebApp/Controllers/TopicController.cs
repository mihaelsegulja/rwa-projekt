using AutoMapper;
using Core.Dtos;
using Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shared.Enums;
using Shared.Exceptions;
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
            TempData["Error"] = "Invalid topic";
            var topics = await _topicService.GetAllTopicsAsync();
            return View("Index", _mapper.Map<List<TopicVm>>(topics));
        }

        try
        {
            var result = await _topicService.AddTopicAsync(vm.Name.Trim());
            TempData["Success"] = result;
        }
        catch (AppException e)
        {
            TempData["Error"] = e.Message;
        }

        return RedirectToAction("Index");
    }

    [HttpPost]
    public async Task<IActionResult> Update(TopicVm vm)
    {
        if (!ModelState.IsValid)
        {
            TempData["Error"] = "Invalid update";
            var topics = await _topicService.GetAllTopicsAsync();
            return View("Index", _mapper.Map<List<TopicVm>>(topics));
        }

        try
        {
            vm.Name = vm.Name.Trim();
            var dto = _mapper.Map<TopicDto>(vm);
            var result = await _topicService.UpdateTopicAsync(dto);
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
            var result = await _topicService.DeleteTopicAsync(id);
            TempData["Success"] = result;
        }
        catch (AppException e)
        {
            TempData["Error"] = e.Message;
        }

        return RedirectToAction("Index");
    }
}
