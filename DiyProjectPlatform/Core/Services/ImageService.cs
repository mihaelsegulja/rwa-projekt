using AutoMapper;
using Core.Context;
using Core.Dtos;
using Core.Interfaces;
using Core.Models;
using Microsoft.EntityFrameworkCore;
using Shared.Enums;
using Shared.Exceptions;

namespace Core.Services;

public class ImageService : IImageService
{
    private readonly DbDiyProjectPlatformContext _dbContext;
    private readonly IMapper _mapper;
    private readonly ILogService _logService;

    public ImageService(DbDiyProjectPlatformContext dbContext, IMapper mapper, ILogService logService)
    {
        _dbContext = dbContext;
        _mapper = mapper;
        _logService = logService;
    }

    public async Task<(byte[]? Bytes, string? ContentType)> GetImageAsync(int id)
    {
        var image = await _dbContext.Images
            .Where(i => i.Id == id)
            .Select(i => i.ImageData)
            .FirstOrDefaultAsync();

        if (image == null || string.IsNullOrWhiteSpace(image))
            return (null, null);

        var bytes = Convert.FromBase64String(image);
        return (bytes, "image/png");
    }

    public async Task AddImagesToProjectAsync(int projectId, List<ImageDto> images)
    {
        if (images == null || images.Count == 0)
            throw new BadRequestException("No images provided for the project");

        var project = await _dbContext.Projects.FindAsync(projectId) 
            ?? throw new NotFoundException($"Project {projectId} not found");

        foreach (var imageDto in images)
        {
            var image = _mapper.Map<Image>(imageDto);
            image.DateAdded = DateTime.UtcNow;

            _dbContext.Images.Add(image);
            await _dbContext.SaveChangesAsync();

            var projectImage = new ProjectImage
            {
                ProjectId = projectId,
                ImageId = image.Id,
                IsMainImage = imageDto.IsMainImage
            };

            _dbContext.ProjectImages.Add(projectImage);
        }

        await _dbContext.SaveChangesAsync();
    }

    public async Task<string> DeleteImageAsync(int id)
    {
        var image = await _dbContext.Images.FindAsync(id) 
            ?? throw new NotFoundException($"Image {id} not found");

        var projectImages = await _dbContext.ProjectImages
        .Where(pi => pi.ImageId == id)
        .ToListAsync();
        _dbContext.ProjectImages.RemoveRange(projectImages);
        _dbContext.Images.Remove(image);

        await _dbContext.SaveChangesAsync();
        await _logService.AddLogAsync($"Image {id} deleted", LogLevel.Info);

        return $"Image {id} successfully deleted";
    }

    public async Task DeleteImagesByProjectIdAsync(int projectId)
    {
        var projectImages = await _dbContext.ProjectImages
            .Where(pi => pi.ProjectId == projectId)
            .ToListAsync();
        if (projectImages.Count == 0) return;

        var imageIds = projectImages.Select(pi => pi.ImageId).ToList();

        _dbContext.ProjectImages.RemoveRange(projectImages);

        var imagesToDelete = await _dbContext.Images
            .Where(img => imageIds.Contains(img.Id))
            .ToListAsync();

        _dbContext.Images.RemoveRange(imagesToDelete);

        await _dbContext.SaveChangesAsync();
        await _logService.AddLogAsync($"Deleted all images for project {projectId}", LogLevel.Info);
    }
}
