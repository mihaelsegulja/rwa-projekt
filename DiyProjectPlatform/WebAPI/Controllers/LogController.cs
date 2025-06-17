using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Core.Interfaces;

namespace WebAPI.Controllers;

[Route("api/log")]
[ApiController]
[Authorize(Roles = nameof(Shared.Enums.UserRole.Admin))]
public class LogController : ControllerBase
{
    private readonly ILogService _logService;
    private readonly IMapper _mapper;
    
    public LogController(ILogService logService, IMapper mapper)
    {
        _logService = logService;
        _mapper = mapper;
    }
    
    [HttpGet("{n}")]
    public async Task<IActionResult> GetLastNLogs(int n = 10)
    {
        var logs = await _logService.GetLastNLogsAsync(n);
        return Ok(logs);
    }
    
    [HttpGet("count")]
    public async Task<IActionResult> GetLogCount()
    {
        var result = await _logService.GetLogCountAsync();
        return Ok(result);
    }
}
