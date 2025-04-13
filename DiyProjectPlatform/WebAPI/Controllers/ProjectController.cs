using Microsoft.AspNetCore.Mvc;
using WebAPI.Models;

namespace WebAPI.Controllers;

[Route("api/project")]
[ApiController]
public class ProjectController : ControllerBase
{
    private readonly DbDiyProjectPlatformContext _dbContext;
    
    public ProjectController(DbDiyProjectPlatformContext dbContext)
    {
        _dbContext = dbContext;
    }
}
