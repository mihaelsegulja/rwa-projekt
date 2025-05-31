using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;

namespace WebApp.Controllers;

public class AuthController : Controller
{
    [HttpGet]
    public IActionResult Login(string? returnUrl)
    {
        var encoded = Uri.EscapeDataString(returnUrl ?? "/");
        var staticLoginUrl = $"http://localhost:5069/login.html?returnUrl={encoded}";
        return Redirect(staticLoginUrl);
    }

    [HttpPost]
    public async Task<IActionResult> Login([FromForm] string token, [FromForm] string? returnUrl)
    {
        if (string.IsNullOrWhiteSpace(token))
            return BadRequest("Missing token");

        var handler = new JwtSecurityTokenHandler();
        JwtSecurityToken jwt;

        try
        {
            jwt = handler.ReadJwtToken(token);
        }
        catch
        {
            return BadRequest("Invalid token");
        }

        var username = jwt.Claims.FirstOrDefault(c => c.Type == "unique_name")?.Value;
        var userId = jwt.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Sub)?.Value;
        var role = jwt.Claims.FirstOrDefault(c => c.Type == "role")?.Value;

        if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(userId) || string.IsNullOrWhiteSpace(role))
            return BadRequest("Missing claims");

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, username),
            new Claim(ClaimTypes.NameIdentifier, userId),
            new Claim(ClaimTypes.Role, role)
        };

        var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        var principal = new ClaimsPrincipal(identity);

        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

        return Redirect(returnUrl ?? "/");
    }

    [HttpGet]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return RedirectToAction("Index", "Home");
    }

    [HttpGet]
    public IActionResult AccessDenied()
    {
        return View();
    }
}
