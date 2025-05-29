using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Core.Dtos;
using Core.Interfaces;

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
        try
        {
            var topics = await _topicService.GetAllTopicsAsync();
            return Ok(topics);
        }
        catch (Exception e)
        {
            return StatusCode(500, e.Message);
        }
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetTopicById(int id)
    {
        try
        {
            var topic = await _topicService.GetTopicByIdAsync(id);
            return topic == null ? NotFound() : Ok(topic);
        }
        catch (Exception e)
        {
            return StatusCode(500, e.Message);
        }
    }

    [HttpPost("add")]
    public async Task<IActionResult> AddTopic(string topic)
    {
        try
        {
            var result = await _topicService.AddTopicAsync(topic);
            return Ok(result);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (Exception e)
        {
            return StatusCode(500, e.Message);
        }
    }

    [HttpPut("update")]
    public async Task<IActionResult> UpdateTopic(TopicDto topic)
    {
        try
        {
            var result = await _topicService.UpdateTopicAsync(topic);
            return result == null ? NotFound() : Ok(result);
        }
        catch (Exception e)
        {
            return StatusCode(500, e.Message);
        }
    }
}
