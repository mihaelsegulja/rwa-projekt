using Microsoft.AspNetCore.Mvc;
using WebAPI.Dtos;
using WebAPI.Models;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace WebAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class LogController : ControllerBase
{
    private readonly DbDiyProjectPlatformContext _dbContext;
    
    public LogController(DbDiyProjectPlatformContext dbContext)
    {
        _dbContext = dbContext;
    }
}
