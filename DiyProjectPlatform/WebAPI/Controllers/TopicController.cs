using AutoMapper;
using Core.Dtos;
using Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers;

[Route("api/topic")]
[ApiController]
[Authorize]
public class TopicController : ControllerBase
{
    private readonly ITopicService _topicService;
    private readonly IMapper _mapper;

    public TopicController(ITopicService topicService, IMapper mapper)
    {
        _topicService = topicService;
        _mapper = mapper;
    }

    [HttpGet("all")]
    public async Task<IActionResult> GetAllTopics()
    {
        var topics = await _topicService.GetAllTopicsAsync();
        return Ok(topics);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetTopicById(int id)
    {
        var topic = await _topicService.GetTopicByIdAsync(id);
        return Ok(topic);
    }

    [HttpPost("add")]
    public async Task<IActionResult> AddTopic(string topic)
    {
        var result = await _topicService.AddTopicAsync(topic);
        return Ok(result);
    }

    [HttpPut("update")]
    public async Task<IActionResult> UpdateTopic(TopicDto topic)
    {
        var result = await _topicService.UpdateTopicAsync(topic);
        return Ok(result);
    }

    [HttpDelete("delete")]
    public async Task<IActionResult> DeleteTopic(int id)
    {
        var result = await _topicService.DeleteTopicAsync(id);
        return Ok(result);
    }
}
