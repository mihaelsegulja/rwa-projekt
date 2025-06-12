using Microsoft.AspNetCore.Mvc;

namespace WebApp.Controllers;

public class ErrorController : Controller
{
    [Route("Error/{statusCode}")]
    public IActionResult HttpStatusCodeHandler(int statusCode)
    {
        ViewBag.StatusCode = statusCode;
        return View("Status");
    }
}
