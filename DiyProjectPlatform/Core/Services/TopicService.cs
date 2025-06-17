using AutoMapper;
using Core.Context;
using Core.Dtos;
using Core.Interfaces;
using Core.Models;
using Microsoft.EntityFrameworkCore;
using Shared.Enums;
using Shared.Exceptions;

namespace Core.Services;

public class TopicService : ITopicService
{
    private readonly DbDiyProjectPlatformContext _dbContext;
    private readonly IMapper _mapper;
    private readonly ILogService _logService;

    public TopicService(DbDiyProjectPlatformContext dbContext, IMapper mapper, ILogService logService)
    {
        _dbContext = dbContext;
        _mapper = mapper;
        _logService = logService;
    }

    public async Task<IEnumerable<TopicDto>> GetAllTopicsAsync()
    {
        var topics = await _dbContext.Topics.ToListAsync();
        return _mapper.Map<IEnumerable<TopicDto>>(topics);
    }

    public async Task<TopicDto> GetTopicByIdAsync(int id)
    {
        var topic = await _dbContext.Topics.FindAsync(id) 
            ?? throw new NotFoundException($"Topic {id} not found");

        return _mapper.Map<TopicDto>(topic);
    }

    public async Task<string> AddTopicAsync(string topic)
    {
        var trimmed = topic.Trim();

        if (await _dbContext.Topics.AnyAsync(t => t.Name == trimmed))
            throw new ConflictException($"Topic '{trimmed}' already exists");

        var newTopic = new Topic { Name = trimmed };
        await _dbContext.Topics.AddAsync(newTopic);
        await _dbContext.SaveChangesAsync();
        await _logService.AddLogAsync($"Topic {trimmed} added", LogLevel.Info);

        return $"Topic '{trimmed}' successfully added";
    }

    public async Task<string> UpdateTopicAsync(TopicDto topicDto)
    {
        var topic = await _dbContext.Topics.FindAsync(topicDto.Id) 
            ?? throw new NotFoundException($"Topic {topicDto.Id} not found");

        topic.Name = topicDto.Name.Trim();
        await _dbContext.SaveChangesAsync();
        await _logService.AddLogAsync($"Topic {topic.Id} updated", LogLevel.Info);

        return $"Topic {topicDto.Id} successfully updated";
    }

    public async Task<string> DeleteTopicAsync(int id)
    {
        var topic = await _dbContext.Topics.FindAsync(id) 
            ?? throw new NotFoundException($"Topic {id} not found");

        _dbContext.Topics.Remove(topic);
        await _dbContext.SaveChangesAsync();
        await _logService.AddLogAsync($"Topic {id} deleted", LogLevel.Info);

        return $"Topic {id} successfully deleted";
    }
}
