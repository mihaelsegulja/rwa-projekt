﻿namespace Core.Dtos;

public class ProjectListDto
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public DateTime DateCreated { get; set; }
    public string TopicName { get; set; }
    public string DifficultyLevel { get; set; }
    public string Username { get; set; }
    public int? MainImageId { get; set; }
}
