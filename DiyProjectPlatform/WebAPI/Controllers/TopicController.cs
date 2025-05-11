using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
    public async Task<ActionResult<IEnumerable<TopicDto>>> GetAllTopics()
    {
        try
        {
            var topics = await _dbContext.Topics.ToListAsync();

            return Ok(_mapper.Map<IEnumerable<TopicDto>>(topics));
        }
        catch (Exception e)
        {
            return StatusCode(500, e.Message);
        }
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<TopicDto>> GetTopicById(int id)
    {
        try
        {
            var topic = await _dbContext.Topics.FindAsync(id);
            if (topic == null)
                return NotFound();
            
            return Ok(_mapper.Map<TopicDto>(topic));
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
            var trimmedTopic = topic.Trim();
            
            if (await _dbContext.Topics.AnyAsync(t => t.Name == trimmedTopic))
                return BadRequest($"Topic {trimmedTopic} already exists");
            
            var newTopic = new Topic
            {
                Name = trimmedTopic,
            };

            await _dbContext.Topics.AddAsync(_mapper.Map<Topic>(newTopic));
            await _dbContext.SaveChangesAsync();
            
            return Ok($"Topic {trimmedTopic} successfully added");
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
            var existingTopic = await _dbContext.Topics.FindAsync(topic.Id);
            if (existingTopic == null)
                return NotFound();

            existingTopic.Name = topic.Name.Trim();
            await _dbContext.SaveChangesAsync();

            return Ok($"Topic {topic.Id} successfully updated");
        }
        catch (Exception e)
        {
            return StatusCode(500, e.Message);
        }
    }
}
