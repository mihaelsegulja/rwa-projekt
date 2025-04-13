namespace WebAPI.Dtos;

public class LogDto
{
    public DateTime? Timestamp { get; set; }

    public string Level { get; set; }

    public string Message { get; set; }
}