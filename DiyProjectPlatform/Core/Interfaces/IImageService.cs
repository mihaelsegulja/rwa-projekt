using Core.Dtos;

namespace Core.Interfaces;

public interface IImageService
{
    Task<(byte[]? Bytes, string? ContentType)> GetImageAsync(int id);
    Task AddImagesToProjectAsync(int projectId, List<ImageDto> images);
    Task<string> DeleteImageAsync(int id);
    Task DeleteImagesByProjectIdAsync(int projectId);
}
