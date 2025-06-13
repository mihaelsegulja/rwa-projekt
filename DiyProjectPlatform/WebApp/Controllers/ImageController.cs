using Core.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace WebApp.Controllers;

public class ImageController : Controller
{
    private readonly IImageService _imageService;

    public ImageController(IImageService imageService)
    {
        _imageService = imageService;
    }

    [HttpGet("Image/Project/{id}")]
    [ResponseCache(Duration = 3600, Location = ResponseCacheLocation.Any)]
    public async Task<IActionResult> ProjectImage(int id)
    {
        var (bytes, contentType) = await _imageService.GetImageAsync(id);

        if (bytes == null || contentType == null)
            return NotFound();

        return File(bytes, contentType);
    }
}
