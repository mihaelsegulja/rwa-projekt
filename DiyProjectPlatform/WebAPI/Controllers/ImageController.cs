using AutoMapper;
using Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers
{
    [Route("api/image")]
    [ApiController]
    [Authorize]
    public class ImageController : ControllerBase
    {
        private readonly IImageService _imageService;
        private readonly IMapper _mapper;

        public ImageController(IImageService imageService, IMapper mapper)
        {
            _imageService = imageService;
            _mapper = mapper;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetImage(int id)
        {
            try
            {
                var (bytes, contentType) = await _imageService.GetImageAsync(id);
                if (bytes == null || contentType == null)
                    return NotFound("Image not found");
                
                return Ok(File(bytes, contentType));
            }
            catch (Exception e)
            {
                return StatusCode(500, e.Message);
            }
        }

        [Authorize(Roles = nameof(Shared.Enums.UserRole.Admin))]
        [HttpDelete("delete")]
        public async Task<IActionResult> DeleteImage(int id)
        {
            try
            {
                var result = await _imageService.DeleteImageAsync(id);
                return Ok(result);
            }
            catch (InvalidOperationException e)
            {
                return NotFound(e.Message);
            }
            catch (Exception e)
            {
                return StatusCode(500, e.Message);
            }
        }
    }
}
