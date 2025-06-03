using AutoMapper;
using Core.Dtos;
using Core.Interfaces;
using Microsoft.AspNetCore.Mvc;
using WebApp.ViewModels;

namespace WebApp.Controllers
{
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
            return View(topics);
        }

        [HttpPost]
        public async Task<IActionResult> Add(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                return BadRequest("Name is required");

            await _topicService.AddTopicAsync(name.Trim());
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> Update(TopicVm vm)
        {
            vm.Name = vm.Name.Trim();
            var dto = _mapper.Map<TopicDto>(vm);
            await _topicService.UpdateTopicAsync(dto);
            return RedirectToAction("Index");
        }
    }
}
