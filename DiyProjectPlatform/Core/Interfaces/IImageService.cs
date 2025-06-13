namespace Core.Interfaces;

public interface IImageService
{
    Task<(byte[]? Bytes, string? ContentType)> GetImageAsync(int id);
}
