using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Core.Dtos;
using Shared.Helpers;
using Core.Interfaces;
using Shared.Exceptions;

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
        if (!ModelState.IsValid)
            return BadRequest("Invalid input");

        var result = await _userService.UserRegisterAsync(registerDto);
        return Ok(result);
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(UserLoginDto loginDto)
    {
        if (!ModelState.IsValid)
            return BadRequest("Invalid input");

        var token = await _userService.UserLoginAsync(loginDto);
        return token == null ? Unauthorized("Incorrect username or password") : Ok(token);
    }

    [Authorize(Roles = nameof(Shared.Enums.UserRole.Admin))]
    [HttpGet("all")]
    public async Task<IActionResult> GetAllUsers(int page = 1, int pageSize = 10)
    {
        var users = await _userService.GetAllUsersAsync(page, pageSize);
        return Ok(users);
    }

    [Authorize]
    [HttpGet("{id}")]
    public async Task<IActionResult> GetUserById(int id)
    {
        var user = await _userService.GetUserByIdAsync(id);
        return Ok(user);
    }

    [Authorize]
    [HttpPut("update-profile")]
    public async Task<IActionResult> UpdateProfile(UserProfileDto profile)
    {
        var userId = ClaimsHelper.GetClaimValueAsInt(User, ClaimTypes.NameIdentifier);
        var result = await _userService.UpdateUserProfileAsync(userId, profile);
        return Ok(result);
    }

    [Authorize]
    [HttpPut("change-password")]
    public async Task<IActionResult> ChangePassword(ChangePasswordDto changePasswordDto)
    {
        var userId = ClaimsHelper.GetClaimValueAsInt(User, ClaimTypes.NameIdentifier);
        var result = await _userService.ChangeUserPasswordAsync(userId, changePasswordDto);
        return result == null ? NotFound("User not found") : Ok(result);
    }

    [Authorize(Roles = nameof(Shared.Enums.UserRole.Admin))]
    [HttpDelete("delete")]
    public async Task<IActionResult> DeleteUser(int id)
    {
        var adminId = ClaimsHelper.GetClaimValueAsInt(User, ClaimTypes.NameIdentifier);
        var result = await _userService.DeleteUserAsync(adminId, id);
        return Ok(result);
    }
}
