using Microsoft.AspNetCore.Mvc;
using WebAPI.Models;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace WebAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ProjectController : ControllerBase
{
    private readonly DbDiyProjectPlatformContext _context;
    
    public ProjectController(DbDiyProjectPlatformContext context)
    {
        _context = context;
    }
}
