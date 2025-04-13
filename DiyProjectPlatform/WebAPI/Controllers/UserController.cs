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
    
    public UserController(DbDiyProjectPlatformContext dbContext, IConfiguration configuration)
    {
        _dbContext = dbContext;
        _configuration = configuration;
    } 
    
    [HttpPost("[action]")]
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
            var user = new User
            {
                Username = registerDto.Username,
                PasswordHash = b64hash,
                PasswordSalt = b64salt,
                FirstName = registerDto.FirstName,
                LastName = registerDto.LastName,
                Email = registerDto.Email,
                Phone = registerDto.Phone
            };

            // Add user and save changes to database
            _dbContext.Add(user);
            _dbContext.SaveChanges();

            return Ok();
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);
        }
    }

    [HttpPost("[action]")]
    public ActionResult Login(UserLoginDto loginDto)
    {
        try
        {
            var genericLoginFail = "Incorrect username or password";

            // Try to get a user from database
            var existingUser = _dbContext.Users.FirstOrDefault(x => x.Username == loginDto.Username);
            if (existingUser == null)
                return Unauthorized(genericLoginFail);

            // Check if password hash matches
            var b64hash = PasswordHashHelper.GetHash(loginDto.Password, existingUser.PasswordSalt);
            if (b64hash != existingUser.PasswordHash)
                return Unauthorized(genericLoginFail);

            // Find user role
            var userRole = _dbContext.UserRoles.FirstOrDefault(ur => existingUser.UserRoleId == ur.Id);
            
            var secureKey = _configuration["JWT:SecureKey"];
            var serializedToken = JwtTokenHelper.CreateToken(secureKey, 60, loginDto.Username, userRole.Name);

            return Ok(serializedToken);
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);
        }
    }
}
