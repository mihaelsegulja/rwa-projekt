using AutoMapper;
using Core.Context;
using Core.Interfaces;
using Microsoft.EntityFrameworkCore;

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
}
