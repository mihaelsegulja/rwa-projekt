using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Core.Dtos;
using Shared.Helpers;
using Core.Interfaces;

namespace WebAPI.Controllers;

[Route("api/user")]
[ApiController]
public class UserController : ControllerBase
{
    private readonly IUserService _userService;
    private readonly IMapper _mapper;

    public UserController(IUserService userService, IMapper mapper)
    {
        _userService = userService;
        _mapper = mapper;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register(UserRegisterDto registerDto)
    {
        try
        {
            var result = await _userService.UserRegisterAsync(registerDto);
            return Ok(result);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
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
            var token = await _userService.UserLoginAsync(loginDto);
            return token == null ? Unauthorized("Incorrect username or password") : Ok(token);
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);
        }
    }

    [Authorize(Roles = nameof(Core.Enums.UserRole.Admin))]
    [HttpGet("all")]
    public async Task<IActionResult> GetAllUsers(int page = 1, int pageSize = 10)
    {
        try
        {
            var users = await _userService.GetAllUsersAsync(page, pageSize);
            return Ok(users);
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);
        }
    }

    [Authorize]
    [HttpGet("{id}")]
    public async Task<IActionResult> GetUserById(int id)
    {
        try
        {
            var user = await _userService.GetUserByIdAsync(id);
            return user == null ? NotFound("User not found") : Ok(user);
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
            var userId = ClaimsHelper.GetClaimValueAsInt(User, ClaimTypes.NameIdentifier);
            var result = await _userService.UpdateUserProfileAsync(userId, profile);
            return result == null ? NotFound("User not found") : Ok(result);
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);
        }
    }
    
    [Authorize(Roles = nameof(Core.Enums.UserRole.Admin))]
    [HttpDelete("delete")]
    public async Task<IActionResult> DeleteUser(int id)
    {
        try
        {
            var adminId = ClaimsHelper.GetClaimValueAsInt(User, ClaimTypes.NameIdentifier);
            var result = await _userService.DeleteUserAsync(adminId, id);
            return result == null ? NotFound("User not found") : Ok(result);
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);
        }
    }

    [Authorize]
    [HttpPut("change-password")]
    public async Task<IActionResult> ChangePassword(ChangePasswordDto changePasswordDto)
    {
        try
        {
            var userId = ClaimsHelper.GetClaimValueAsInt(User, ClaimTypes.NameIdentifier);
            var result = await _userService.ChangeUserPasswordAsync(userId, changePasswordDto);
            return result == null ? NotFound("User not found") : Ok(result);
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);
        }
    }
}
