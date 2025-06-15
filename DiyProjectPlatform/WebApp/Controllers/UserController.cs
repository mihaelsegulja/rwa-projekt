using AutoMapper;
using Core.Dtos;
using Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shared.Helpers;
using System.Security.Claims;
using WebApp.ViewModels;

namespace WebApp.Controllers;

[Authorize]
public class UserController : Controller
{
    private readonly IUserService _userService;
    private readonly IMapper _mapper;

    public UserController(IUserService userService, IMapper mapper)
    {
        _userService = userService;
        _mapper = mapper;
    }

    [HttpGet]
    public async Task<IActionResult> UpdateProfile()
    {
        var userId = ClaimsHelper.GetClaimValueAsInt(User, ClaimTypes.NameIdentifier);
        var profile = await _userService.GetUserByIdAsync(userId);
        return View(_mapper.Map<UserProfileVm>(profile));
    }

    [HttpPost]
    public async Task<IActionResult> UpdateProfile([FromForm] UserProfileVm profile)
    {
        if (!ModelState.IsValid)
        {
            var errors = ModelState.ToDictionary(
                kvp => kvp.Key,
                kvp => kvp.Value?.Errors.FirstOrDefault()?.ErrorMessage
            );
            return BadRequest(errors);
        }

        var userId = ClaimsHelper.GetClaimValueAsInt(User, ClaimTypes.NameIdentifier);
        var dto = _mapper.Map<UserProfileDto>(profile);
        var result = await _userService.UpdateUserProfileAsync(userId, dto);

        if (result == null)
            return BadRequest(new { General = "Update failed" });

        return Ok("Profile updated successfully.");
    }

    [HttpGet]
    public IActionResult ChangePassword()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> ChangePassword([FromForm] ChangePasswordVm vm)
    {
        if (!ModelState.IsValid)
        {
            var errors = ModelState.ToDictionary(
                kvp => kvp.Key,
                kvp => kvp.Value?.Errors.FirstOrDefault()?.ErrorMessage
            );
            return BadRequest(errors);
        }

        var userId = ClaimsHelper.GetClaimValueAsInt(User, ClaimTypes.NameIdentifier);
        var dto = _mapper.Map<ChangePasswordDto>(vm);
        var result = await _userService.ChangeUserPasswordAsync(userId, dto);

        if (result == null)
            return BadRequest(new { General = "Password change failed" });

        return Ok("Password successfully changed.");
    }
}
