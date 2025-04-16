using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebAPI.Dtos;
using WebAPI.Models;

namespace WebAPI.Controllers;

[Route("api/log")]
[ApiController]
public class LogController : ControllerBase
{
    private readonly DbDiyProjectPlatformContext _dbContext;
    private readonly IMapper _mapper;
    
    public LogController
    (DbDiyProjectPlatformContext dbContext, IMapper mapper)
    {
        _dbContext = dbContext;
        _mapper = mapper;
    }
    
    /// <summary>
    /// Get last N logs
    /// </summary>
    /// <param name="n">Number of logs</param>
    [Authorize(Roles = "Admin")]
    [HttpGet("get/{n}")]
    public async Task<ActionResult<IEnumerable<LogDto>>> GetLastNLogs(int n)
    {
        try
        {
            var logs = await _dbContext.Logs
                .OrderByDescending(l => l.Timestamp)
                .Take(n)
                .ToListAsync();

            return Ok(_mapper.Map<IEnumerable<LogDto>>(logs));
        }
        catch (Exception e)
        {
            return StatusCode(500, e.Message);
        }
    }

    /// <summary>
    /// Get total log count
    /// </summary>
    [Authorize(Roles = "Admin")]
    [HttpGet("count")]
    public async Task<ActionResult<int>> GetLogCount()
    {
        try
        {
            var result = await _dbContext.Logs.CountAsync();
            
            return Ok(result);
        }
        catch (Exception e)
        {
            return StatusCode(500, e.Message);
        }
    }
}
