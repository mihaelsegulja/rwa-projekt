using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using WebAPI.Dtos;
using WebAPI.Helpers;
using WebAPI.Models;

namespace WebAPI.Controllers;

[Route("api/user")]
[ApiController]
public class UserController : ControllerBase
{
    private readonly DbDiyProjectPlatformContext _dbContext;
    private readonly IMapper _mapper;

    public UserController(DbDiyProjectPlatformContext dbContext, IMapper mapper)
    {
        _dbContext = dbContext;
        _mapper = mapper;
    }

    [HttpPost("register")]
    public ActionResult Register(UserRegisterDto registerDto)
    {
        try
        {
            var trimmedUsername = registerDto.Username.Trim();
            if (_dbContext.Users.Any(x => x.Username.Equals(trimmedUsername)))
                return BadRequest($"Username {trimmedUsername} already exists");

            var b64Salt = PasswordHashHelper.GetSalt();
            var b64Hash = PasswordHashHelper.GetHash(registerDto.Password, b64Salt);

            var user = _mapper.Map<User>(registerDto);
            user.Username = trimmedUsername;
            user.PasswordHash = b64Hash;
            user.PasswordSalt = b64Salt;
            user.IsActive = true;

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

            var existingUser = _dbContext.Users.FirstOrDefault(x => x.IsActive && x.Username == loginDto.Username);
            if (existingUser == null)
                return Unauthorized(genericLoginFail);

            var b64Hash = PasswordHashHelper.GetHash(loginDto.Password, existingUser.PasswordSalt);
            if (b64Hash != existingUser.PasswordHash)
                return Unauthorized(genericLoginFail);

            var userRole = _dbContext.UserRoles.FirstOrDefault(ur => existingUser.UserRoleId == ur.Id);

            var serializedToken = JwtTokenHelper.CreateToken(loginDto.Username, existingUser.Id.ToString(), userRole.Name);

            return Ok(serializedToken);
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

    [Authorize]
    [HttpGet("{id}")]
    public ActionResult<UserDto> GetUserById(int id)
    {
        try
        {
            var user = _dbContext.Users.FirstOrDefault(x => x.Id == id && x.IsActive);
            if (user == null)
                return NotFound("User not found");
            
            return Ok(_mapper.Map<UserDto>(user));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);
        }
    }

    // TODO: Add UpdateProfile where every user property can be updated, except password
    // Password update will have to go in i.e. UpdatePassword
    
    [Authorize]
    [HttpPut("profilepicture")]
    public ActionResult UpdateProfilePicture([FromBody] string profilePicture)
    {
        try
        {
            var currentUserId = int.Parse(User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value);
            var user = _dbContext.Users.FirstOrDefault(x => x.Id == currentUserId);
            if (user == null)
                return NotFound("User not found");
            
            user.ProfilePicture = profilePicture;
            _dbContext.SaveChanges();
            
            return Ok("Profile picture updated");
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);
        }
    }
    
    [Authorize(Roles = "Admin")]
    [HttpDelete("delete")]
    public ActionResult DeleteUser(int id)
    {
        try
        {
            // Is this needed???
            var currentUserId = int.Parse(User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value);
            if (currentUserId == id)
                return BadRequest("You cannot delete your own account");

            var user = _dbContext.Users.FirstOrDefault(x => x.Id == id);
            if (user == null)
                return NotFound("User not found");

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
}
