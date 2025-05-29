namespace Core.Dtos;

public class ImageDto
{
    public int Id { get; set; }
    public string ImageData { get; set; }
    public string? Description { get; set; }
    public bool IsMainImage { get; set; }
}
