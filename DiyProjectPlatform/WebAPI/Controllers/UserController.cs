using AutoMapper;
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
            var expirationTime = int.Parse(_configuration["JWT:ExpiryInMinutes"]);
            var serializedToken = JwtTokenHelper.CreateToken(secureKey, expirationTime, loginDto.Username, userRole.Name);

            return Ok(serializedToken);
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);
        }
    }
}
