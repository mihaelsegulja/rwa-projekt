using AutoMapper;
using Core.Dtos;
using Core.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using WebApp.ViewModels;

namespace WebApp.Controllers;

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
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var profile = await _userService.GetUserByIdAsync(userId);
        return View(_mapper.Map<UserProfileVm>(profile));
    }

    [HttpPost]
    public async Task<IActionResult> UpdateProfile(UserProfileVm profile)
    {
        if (!ModelState.IsValid)
            return View(profile);

        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var userProfile = _mapper.Map<UserProfileDto>(profile);
        var response = await _userService.UpdateUserProfileAsync(userId, userProfile);
        TempData["Success"] = response;
        return RedirectToAction("UpdateProfile");
    }

    [HttpGet]
    public IActionResult ChangePassword()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> ChangePassword(ChangePasswordVm vm)
    {
        if (!ModelState.IsValid)
            return View(vm);

        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var dto = _mapper.Map<ChangePasswordDto>(vm);
        var result = await _userService.ChangeUserPasswordAsync(userId, dto);
        if (result != null)
        {
            ModelState.AddModelError(string.Empty, "Password change failed.");
            return View(dto);
        }

        TempData["Success"] = "Password changed.";
        return RedirectToAction("UpdateProfile");
    }
}
