using AutoMapper;
using Core.Context;
using Core.Dtos;
using Core.Interfaces;
using Core.Models;
using Microsoft.EntityFrameworkCore;
using Shared.Enums;

namespace Core.Services;

public class LogService : ILogService
{
    private readonly DbDiyProjectPlatformContext _dbContext;
    private readonly IMapper _mapper;

    public LogService(DbDiyProjectPlatformContext dbContext, IMapper mapper)
    {
        _dbContext = dbContext;
        _mapper = mapper;
    }

    public async Task<IEnumerable<LogDto>> GetLastNLogsAsync(int n)
    {
        var logs = await _dbContext.Logs
                .OrderByDescending(l => l.Timestamp)
                .Take(n)
                .ToListAsync();

        return _mapper.Map<IEnumerable<LogDto>>(logs);
    }

    public async Task<int> GetLogCountAsync()
    {
        var result = await _dbContext.Logs.CountAsync();
        return result;
    }

    public async Task AddLogAsync(string message, LogLevel level = LogLevel.None)
    {
        var logDto = new LogDto
        {
            Message = message,
            Level = level.ToString(),
            Timestamp = DateTime.UtcNow
        };

        var log = _mapper.Map<Log>(logDto);
        _dbContext.Logs.Add(log);
        await _dbContext.SaveChangesAsync();
    }
}
