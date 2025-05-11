using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
    public async Task<IActionResult> Register(UserRegisterDto registerDto)
    {
        try
        {
            var trimmedUsername = registerDto.Username.Trim();
            if (await _dbContext.Users.AnyAsync(x => x.Username.Equals(trimmedUsername)))
                return BadRequest($"Username {trimmedUsername} already exists");

            var b64Salt = PasswordHashHelper.GetSalt();
            var b64Hash = PasswordHashHelper.GetHash(registerDto.Password, b64Salt);

            var user = _mapper.Map<User>(registerDto);
            user.Username = trimmedUsername;
            user.PasswordHash = b64Hash;
            user.PasswordSalt = b64Salt;
            user.IsActive = true;

            await _dbContext.AddAsync(user);
            await _dbContext.SaveChangesAsync();

            return Ok("Success");
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);
        }
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(UserLoginDto loginDto)
    {
        try
        {
            var genericLoginFail = "Incorrect username or password";

            var existingUser = await _dbContext.Users.FirstOrDefaultAsync(x => x.IsActive && x.Username == loginDto.Username);
            if (existingUser == null)
                return Unauthorized(genericLoginFail);

            var b64Hash = PasswordHashHelper.GetHash(loginDto.Password, existingUser.PasswordSalt);
            if (b64Hash != existingUser.PasswordHash)
                return Unauthorized(genericLoginFail);

            var userRole = await _dbContext.UserRoles.FirstOrDefaultAsync(ur => existingUser.UserRoleId == ur.Id);

            var serializedToken = JwtTokenHelper.CreateToken(loginDto.Username, existingUser.Id.ToString(), userRole.Name);

            return Ok(serializedToken);
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);
        }
    }

    [Authorize(Roles = nameof(Enums.UserRole.Admin))]
    [HttpGet("all")]
    public async Task<ActionResult<IEnumerable<UserDto>>> GetAllUsers(int page = 1, int pageSize = 10)
    {
        try
        {
            var users = await _dbContext.Users
                .Where(u => u.IsActive)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return Ok(_mapper.Map<IEnumerable<UserDto>>(users));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);
        }
    }

    [Authorize]
    [HttpGet("{id}")]
    public async Task<ActionResult<UserDto>> GetUserById(int id)
    {
        try
        {
            var user = await _dbContext.Users.FirstOrDefaultAsync(x => x.Id == id && x.IsActive);
            if (user == null)
                return NotFound("User not found");
            
            return Ok(_mapper.Map<UserDto>(user));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);
        }
    }

    [Authorize]
    [HttpPut("update-profile")]
    public async Task<IActionResult> UpdateProfile(UserProfileDto profile)
    {
        try
        {
            var currentUserId = ClaimsHelper.GetClaimValueAsInt(User, ClaimTypes.NameIdentifier);

            var existingUser = await _dbContext.Users.FirstOrDefaultAsync(x => x.Id == currentUserId);
            if (existingUser == null)
                return NotFound("User not found");
            
            existingUser.Username = profile.Username;
            existingUser.FirstName = profile.FirstName;
            existingUser.LastName = profile.LastName;
            existingUser.Email = profile.Email;
            existingUser.Phone = profile.Phone;
            existingUser.ProfilePicture = profile.ProfilePicture;
            
            await _dbContext.SaveChangesAsync();

            return Ok("Profile updated");
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);
        }
    }
    
    [Authorize(Roles = nameof(Enums.UserRole.Admin))]
    [HttpDelete("delete")]
    public async Task<IActionResult> DeleteUser(int id)
    {
        try
        {
            // Is this needed???
            var currentUserId = ClaimsHelper.GetClaimValueAsInt(User, ClaimTypes.NameIdentifier);
            if (currentUserId == id)
                return BadRequest("You cannot delete your own account");

            var user = await _dbContext.Users.FirstOrDefaultAsync(x => x.Id == id);
            if (user == null)
                return NotFound("User not found");

            user.IsActive = false;
            user.DateDeleted = DateTime.UtcNow;
            await _dbContext.SaveChangesAsync();

            return Ok("User has been deleted");
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);
        }
    }
}
