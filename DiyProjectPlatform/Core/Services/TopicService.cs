using AutoMapper;
using Core.Context;
using Core.Dtos;
using Core.Interfaces;
using Core.Models;
using Microsoft.EntityFrameworkCore;

namespace Core.Services;

public class TopicService : ITopicService
{
    public readonly DbDiyProjectPlatformContext _dbContext;
    private readonly IMapper _mapper;

    public TopicService(DbDiyProjectPlatformContext dbContext, IMapper mapper)
    {
        _dbContext = dbContext;
        _mapper = mapper;
    }

    public async Task<IEnumerable<TopicDto>> GetAllTopicsAsync()
    {
        var topics = await _dbContext.Topics.ToListAsync();
        return _mapper.Map<IEnumerable<TopicDto>>(topics);
    }

    public async Task<TopicDto?> GetTopicByIdAsync(int id)
    {
        var topic = await _dbContext.Topics.FindAsync(id);
        return topic == null ? null : _mapper.Map<TopicDto>(topic);
    }

    public async Task<string> AddTopicAsync(string topic)
    {
        var trimmed = topic.Trim();

        if (await _dbContext.Topics.AnyAsync(t => t.Name == trimmed))
            throw new InvalidOperationException($"Topic '{trimmed}' already exists");

        var newTopic = new Topic { Name = trimmed };
        await _dbContext.Topics.AddAsync(newTopic);
        await _dbContext.SaveChangesAsync();

        return $"Topic '{trimmed}' successfully added";
    }

    public async Task<string?> UpdateTopicAsync(TopicDto topicDto)
    {
        var existing = await _dbContext.Topics.FindAsync(topicDto.Id);
        if (existing == null) return null;

        existing.Name = topicDto.Name.Trim();
        await _dbContext.SaveChangesAsync();
        return $"Topic {topicDto.Id} successfully updated";
    }
}
