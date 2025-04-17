using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebAPI.Dtos;
using WebAPI.Models;
using WebAPI.Security;

namespace WebAPI.Controllers;

[Route("api/user")]
[ApiController]
public class UserController : ControllerBase
{
    private readonly DbDiyProjectPlatformContext _dbContext;
    private readonly IConfiguration _configuration;
    private readonly IMapper _mapper;

    public UserController(DbDiyProjectPlatformContext dbContext, IConfiguration configuration, IMapper mapper)
    {
        _dbContext = dbContext;
        _configuration = configuration;
        _mapper = mapper;
    } 
    
    [HttpPost("register")]
    public ActionResult Register(UserRegisterDto registerDto)
    {
        try
        {
            // Check if there is such a username in the database already
            var trimmedUsername = registerDto.Username.Trim();
            if (_dbContext.Users.Any(x => x.Username.Equals(trimmedUsername)))
                return BadRequest($"Username {trimmedUsername} already exists");

            // Hash the password
            var b64salt = PasswordHashHelper.GetSalt();
            var b64hash = PasswordHashHelper.GetHash(registerDto.Password, b64salt);

            // Create user from DTO and hashed password
            var user = _mapper.Map<User>(registerDto);
            user.PasswordHash = b64hash;
            user.PasswordSalt = b64salt;
            user.IsActive = true;

            // Add user and save changes to database
            _dbContext.Add(user);
            _dbContext.SaveChanges();

            return Ok("Success");
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);
        }
    }

    [HttpPost("login")]
    public ActionResult Login(UserLoginDto loginDto)
    {
        try
        {
            var genericLoginFail = "Incorrect username or password";

            // Try to get a user from database
            var existingUser = _dbContext.Users.FirstOrDefault(x => x.Username == loginDto.Username && x.IsActive);
            if (existingUser == null)
                return Unauthorized(genericLoginFail);

            // Check if password hash matches
            var b64hash = PasswordHashHelper.GetHash(loginDto.Password, existingUser.PasswordSalt);
            if (b64hash != existingUser.PasswordHash)
                return Unauthorized(genericLoginFail);

            // Find user role
            var userRole = _dbContext.UserRoles.FirstOrDefault(ur => existingUser.UserRoleId == ur.Id);

            var serializedToken = JwtTokenHelper.CreateToken(loginDto.Username, existingUser.Id.ToString(), userRole.Name);

            return Ok(serializedToken);
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);
        }
    }

    [Authorize]
    [HttpDelete("delete")]
    public ActionResult DeleteUser(int id)
    {
        try
        {
            // Get the ID of the currently authenticated user
            var currentUserId = int.Parse(User.Claims.First(c => c.Type == "sub").Value);

            // Check if the user is trying to delete themselves
            if (currentUserId == id)
                return BadRequest("You cannot delete your own account");

            // Find the user by ID
            var user = _dbContext.Users.FirstOrDefault(x => x.Id == id);
            if (user == null)
                return NotFound("User not found");

            // Perform a soft delete by setting IsActive to false and setting DateDeleted
            user.IsActive = false;
            user.DateDeleted = DateTime.UtcNow;

            _dbContext.SaveChanges();

            return Ok("User has been deleted");
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);
        }
    }
    
    [Authorize(Roles = "Admin")]
    [HttpGet("all")]
    public ActionResult<IEnumerable<UserDto>> GetAllUsers(int page = 1, int pageSize = 10)
    {
        try
        {
            var users = _dbContext.Users
                .Where(u => u.IsActive)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();
            return Ok(_mapper.Map<IEnumerable<UserDto>>(users));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);
        }
    }
}
