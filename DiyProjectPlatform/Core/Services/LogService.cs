using AutoMapper;
using Core.Context;
using Core.Dtos;
using Core.Interfaces;
using Microsoft.EntityFrameworkCore;

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
}
