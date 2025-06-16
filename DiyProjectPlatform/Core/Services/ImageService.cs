using AutoMapper;
using Core.Context;
using Core.Dtos;
using Core.Interfaces;
using Core.Models;
using Microsoft.EntityFrameworkCore;
using Shared.Enums;

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
        var image = await _dbContext.Images.FindAsync(id);
        if (image == null)
            throw new InvalidOperationException($"Image {id} not found");

        _dbContext.ProjectImages.RemoveRange(
            _dbContext.ProjectImages.Where(pi => pi.ImageId == id)
        );
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
        if (projectImages == null)
            throw new InvalidOperationException($"Project does not have images");

        var imageIds = projectImages.Select(pi => pi.ImageId).ToList();

        _dbContext.ProjectImages.RemoveRange(projectImages);

        var imagesToDelete = await _dbContext.Images
            .Where(img => imageIds.Contains(img.Id))
            .ToListAsync();

        _dbContext.Images.RemoveRange(imagesToDelete);

        await _dbContext.SaveChangesAsync();
    }
}
