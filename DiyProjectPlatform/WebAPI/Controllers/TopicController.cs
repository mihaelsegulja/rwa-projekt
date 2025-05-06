using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebAPI.Dtos;
using WebAPI.Models;

namespace WebAPI.Controllers;

[Route("api/topic")]
[ApiController]
[Authorize]
public class TopicController : ControllerBase
{
    private readonly DbDiyProjectPlatformContext _dbContext;
    private readonly IMapper _mapper;

    public TopicController(DbDiyProjectPlatformContext dbContext, IMapper mapper)
    {
        _dbContext = dbContext;
        _mapper = mapper;
    }

    [HttpGet("all")]
    public IActionResult GetAllTopics(int page = 1, int pageSize = 10)
    {
        try
        {
            var topics = _dbContext.Topics
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            return Ok(topics);
        }
        catch (Exception e)
        {
            return StatusCode(500, e.Message);
        }
    }

    [HttpGet("{id}")]
    public IActionResult GetTopicById(int id)
    {
        try
        {
            var topic = _dbContext.Topics.Find(id);
            if (topic == null)
                return NotFound();
            
            return Ok(topic);
        }
        catch (Exception e)
        {
            return StatusCode(500, e.Message);
        }
    }

    [HttpPost("add")]
    public IActionResult AddTopic([FromBody] TopicDto topic)
    {
        try
        {
            var trimmedTopic = topic.Name.Trim();
            
            if (_dbContext.Topics.Any(t => t.Name == trimmedTopic))
                return BadRequest($"Topic {trimmedTopic} already exists");
            
            topic.Name = trimmedTopic;
            _dbContext.Topics.Add(_mapper.Map<Topic>(topic));
            _dbContext.SaveChanges();
            
            return Ok($"Topic {trimmedTopic} successfully added");
        }
        catch (Exception e)
        {
            return StatusCode(500, e.Message);
        }
    }
}
