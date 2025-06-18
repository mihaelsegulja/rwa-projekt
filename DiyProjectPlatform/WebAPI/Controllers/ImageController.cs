using AutoMapper;
using Core.Dtos;
using Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
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
            var (bytes, contentType) = await _imageService.GetImageAsync(id);
            if (bytes == null || contentType == null)
                return NotFound("Image not found");
                
            return Ok(File(bytes, contentType));
        }

        [Authorize(Roles = nameof(Shared.Enums.UserRole.Admin))]
        [HttpPost("upload")]
        public async Task<IActionResult> UpladImagesForProject(int projectId, [FromBody] List<ImageDto> images)
        {
            await _imageService.AddImagesToProjectAsync(projectId, images);
            return Ok($"Successfully added {images.Count} images to project {projectId}");
        }

        [Authorize(Roles = nameof(Shared.Enums.UserRole.Admin))]
        [HttpDelete("delete")]
        public async Task<IActionResult> DeleteImage(int id)
        {
            var result = await _imageService.DeleteImageAsync(id);
            return Ok(result);
        }

        [Authorize(Roles = nameof(Shared.Enums.UserRole.Admin))]
        [HttpDelete("delete-project-images")]
        public async Task<IActionResult> DeleteImagesByProjectId(int projectId)
        {
            await _imageService.DeleteImagesByProjectIdAsync(projectId);
            return Ok($"Successfully deleted all images for project {projectId}");
        }
    }
}
